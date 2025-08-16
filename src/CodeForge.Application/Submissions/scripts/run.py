import json
import os
import shutil
import subprocess
import sys
import tempfile

import requests


def parse_output(output_str):
    time_ms = 0
    mem_kb = 0
    exit_code = 1
    for line in output_str.splitlines():
        if line.startswith("TIME:"):
            time_ms = int(line.split(":")[1])
        elif line.startswith("MEM:"):
            mem_kb = int(line.split(":")[1])
        elif line.startswith("EXIT:"):
            exit_code = int(line.split(":")[1])
    return time_ms, mem_kb, exit_code


def main():
    if len(sys.argv) != 5:
        print("Usage: python judge.py <code_path> <language> <input_url> <output_url>", file=sys.stderr)
        sys.exit(1)

    code_path = sys.argv[1]
    lang = sys.argv[2].lower()
    input_url = sys.argv[3]
    output_url = sys.argv[4]

    if lang not in ['cpp', 'python', 'c#']:
        print("Unsupported language", file=sys.stderr)
        sys.exit(1)

    if not os.path.exists(code_path):
        print("Code file not found", file=sys.stderr)
        sys.exit(1)

    time_limit_ms = 2000
    memory_limit_kb = 256 * 1024  # 256 MB

    with tempfile.TemporaryDirectory() as tmpdir:
        temp_code_path = os.path.join(tmpdir, 'code')
        input_path = os.path.join(tmpdir, 'input.txt')
        expected_path = os.path.join(tmpdir, 'expected.txt')
        user_out_path = os.path.join(tmpdir, 'user_out.txt')

        shutil.copy(code_path, temp_code_path)

        download(input_url, input_path)
        download(output_url, expected_path)

        if lang == 'cpp':
            ext = 'cpp'
            container = 'cpp-env'
            compile_cmd = 'g++ /workspace/user.cpp -o /workspace/user -fsanitize=address -fsanitize=undefined 2> /workspace/compile_err.txt'
            run_cmd = './user'
        elif lang == 'python':
            ext = 'py'
            container = 'python-env'
            compile_cmd = 'true'
            run_cmd = 'python /workspace/user.py'
        elif lang == 'c#':
            ext = 'cs'
            container = 'csharp-env'
            compile_cmd = 'dotnet new console -o /workspace/app --force && cp /workspace/user.cs /workspace/app/Program.cs && dotnet build /workspace/app 2> /workspace/compile_err.txt'
            run_cmd = 'dotnet run --project /workspace/app --no-build'

        src_path = os.path.join(tmpdir, f'user.{ext}')
        os.rename(temp_code_path, src_path)

        bash_cmd = (
            f'{compile_cmd} ; '
            f'comp_status=$? ; '
            f'if [ $comp_status -ne 0 ]; then echo "CompilationError"; exit $comp_status; fi ; '
            f'start=$(date +%s%N) ; '
            f'( {run_cmd} < /workspace/input.txt > /workspace/user_out.txt 2> /workspace/runtime_err.txt ) ; '
            f'exitcode=$? ; '
            f'end=$(date +%s%N) ; '
            f'mem=$(cat /sys/fs/cgroup/memory.peak 2>/dev/null || echo 0) ; '
            f'echo "TIME:$(( (end - start) / 1000000 ))" ; '
            f'echo "MEM:$(( mem / 1024 ))" ; '
            f'echo "EXIT:$exitcode"'
        )

        docker_cmd = [
            'docker', 'exec',
            '-w', '/workspace',
            container, 'bash', '-c', bash_cmd
        ]

        verdict = "Pending"
        exec_time = 0
        used_mem = 0

        try:
            # Copy files into container before exec
            subprocess.run(['docker', 'cp', f'{tmpdir}/.', f'{container}:/workspace'], check=True)

            p = subprocess.run(docker_cmd, capture_output=True, timeout=5)
            output = p.stdout.decode('utf-8', errors='ignore')

            if p.returncode == 137:
                verdict = "MemoryLimitExceeded"
            elif "CompilationError" in output:
                verdict = "CompilationError"
            else:
                exec_time, used_mem, exit_code = parse_output(output)
                if exit_code != 0:
                    verdict = "RuntimeError"
                else:
                    # Copy back user output
                    subprocess.run(['docker', 'cp', f'{container}:/workspace/user_out.txt', user_out_path], check=True)

                    with open(user_out_path, 'r', encoding='utf-8', errors='ignore') as f:
                        user_output = f.read().rstrip()
                    with open(expected_path, 'r', encoding='utf-8', errors='ignore') as f:
                        expected_output = f.read().rstrip()
                    if user_output == expected_output:
                        verdict = "Accepted"
                    else:
                        verdict = "WrongAnswer"

                if exec_time > time_limit_ms:
                    verdict = "TimeLimitExceeded"

        except subprocess.TimeoutExpired:
            verdict = "TimeLimitExceeded"
        except Exception:
            verdict = "RuntimeError"

        result = {
            "overall_verdict": verdict,
            "execution_time_ms": exec_time if verdict in ["Accepted", "WrongAnswer", "RuntimeError"] else 0,
            "used_memory_kb": used_mem if verdict in ["Accepted", "WrongAnswer", "RuntimeError"] else 0
        }

        print(json.dumps(result))


def download(url, dest):
    r = requests.get(url)
    r.raise_for_status()
    with open(dest, 'wb') as f:
        f.write(r.content)


if __name__ == "__main__":
    main()