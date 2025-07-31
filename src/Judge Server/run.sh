#!/bin/sh

# Configuration
MAX_EXECUTION_TIME=${MAX_EXECUTION_TIME:-1}  # Default: 1 second
MAX_MEMORY_KB=${MAX_MEMORY_KB:-256000}       # Default: 256MB

# File paths
COMPILE_ERROR_FILE="compile_error.txt"
RUNTIME_ERROR_FILE="runtime_error.txt"
RESULT_FILE="result.txt"
TIME_STATS_FILE="time_stats.txt"

# Output verdict with JSON format
print_verdict() {
    local verdict=$1
    local time=$2
    local memory=$3
    
    printf '{"Verdict": "%s", "ExecutionTime": %s, "MemoryUsed": %s}\n' "$verdict" "$time" "$memory"
}

# Download submission files
download_files() {
    local code_url=$1
    local input_url=$2
    local output_url=$3

    curl -s -o source_code "$code_url"
    curl -s -o input.txt "$input_url"
    curl -s -o expected_output.txt "$output_url"
    
    # Verify downloads
    if [ ! -f source_code ] || [ ! -f input.txt ] || [ ! -f expected_output.txt ]; then
        print_verdict "SystemError" 0 0
        exit 1
    fi
}

# Detect and compile source code
setup_language() {
    file_type=$(file -b source_code)
    
    # Python detection
    if echo "$file_type" | grep -qi "python script"; then
        mv source_code code.py
        echo "python3 code.py"
        return 0
    fi
    
    # C++ detection
    if echo "$file_type" | grep -qi "c++ source"; then
        mv source_code code.cpp
        if ! g++ -o program code.cpp 2> "$COMPILE_ERROR_FILE"; then
            return 1
        fi
        echo "./program"
        return 0
    fi
    
    # C detection
    if echo "$file_type" | grep -qi "c source"; then
        mv source_code code.c
        if ! gcc -o program code.c 2> "$COMPILE_ERROR_FILE"; then
            return 1
        fi
        echo "./program"
        return 0
    fi
    
    # Java detection
    if echo "$file_type" | grep -qi "java source"; then
        mv source_code Code.java
        if ! javac Code.java 2> "$COMPILE_ERROR_FILE"; then
            return 1
        fi
        echo "java Code"
        return 0
    fi
    
    # Fallback to extension-based detection
    case "$(basename source_code)" in
        *.py)  mv source_code code.py; echo "python3 code.py" ;;
        *.cpp) mv source_code code.cpp
               g++ -o program code.cpp 2> "$COMPILE_ERROR_FILE" || return 1
               echo "./program" ;;
        *.c)   mv source_code code.c
               gcc -o program code.c 2> "$COMPILE_ERROR_FILE" || return 1
               echo "./program" ;;
        *.java) mv source_code Code.java
                javac Code.java 2> "$COMPILE_ERROR_FILE" || return 1
                echo "java Code" ;;
        *)     return 1 ;;
    esac
}

# Convert seconds to milliseconds
seconds_to_milliseconds() {
    echo "$1" | awk '{printf "%.0f", $1 * 1000}'
}

# Run program with resource limits
run_with_limits() {
    local cmd=$1
    local time_format="%e %U %S %M"  # real_time user_time sys_time max_memory_kb
    
    # Set resource limits
    if command -v ulimit >/dev/null 2>&1; then
        ulimit -v $MAX_MEMORY_KB 2>/dev/null || true
    fi
    
    # Execute with timeout and resource monitoring
    local exit_code=0
    timeout "$MAX_EXECUTION_TIME" /usr/bin/time -f "$time_format" \
        sh -c "$cmd < input.txt > $RESULT_FILE 2> $RUNTIME_ERROR_FILE" \
        2> "$TIME_STATS_FILE" || exit_code=$?
    
    # Get execution stats
    local real_time="0.000"
    local max_memory_kb="0"
    if [ -f "$TIME_STATS_FILE" ] && [ -s "$TIME_STATS_FILE" ]; then
        read real_time _ _ max_memory_kb < "$TIME_STATS_FILE" 2>/dev/null || true
    fi
    
    local execution_time_ms=$(seconds_to_milliseconds "$real_time")
    
    # Handle execution verdicts
    case $exit_code in
        124) # Time Limit Exceeded
            print_verdict "TimeLimitExceeded" \
                        "$(seconds_to_milliseconds "$MAX_EXECUTION_TIME")" \
                        "$max_memory_kb"
            return 1
            ;;
        137) # Memory Limit Exceeded
            print_verdict "MemoryLimitExceeded" "$execution_time_ms" "$MAX_MEMORY_KB"
            return 1
            ;;
        0)   # Successful execution
            if [ "$max_memory_kb" -gt "$MAX_MEMORY_KB" ] 2>/dev/null; then
                print_verdict "MemoryLimitExceeded" "$execution_time_ms" "$max_memory_kb"
                return 1
            fi
            
            # Compare output
            if diff -Z -B "$RESULT_FILE" expected_output.txt >/dev/null 2>&1; then
                print_verdict "Accepted" "$execution_time_ms" "$max_memory_kb"
            else
                print_verdict "WrongAnswer" "$execution_time_ms" "$max_memory_kb"
            fi
            ;;
        *)   # Runtime Error
            print_verdict "RuntimeError" "$execution_time_ms" "$max_memory_kb"
            return 1
            ;;
    esac
}

# Cleanup temporary files
cleanup() {
    rm -f source_code code.py code.cpp code.c Code.java Code.class program
    rm -f input.txt expected_output.txt "$RESULT_FILE"
    rm -f "$COMPILE_ERROR_FILE" "$RUNTIME_ERROR_FILE" "$TIME_STATS_FILE"
}

# Main execution
trap cleanup EXIT

if [ "$#" -ne 3 ]; then
    echo "Usage: $0 <code_url> <input_url> <output_url>"
    echo "Environment variables:"
    echo "  MAX_EXECUTION_TIME - Maximum execution time in seconds (default: 1)"
    echo "  MAX_MEMORY_KB - Maximum memory usage in KB (default: 256000)"
    exit 1
fi

download_files "$1" "$2" "$3"
exec_cmd=$(setup_language)
if [ $? -ne 0 ]; then
    print_verdict "CompilationError" 0 0
    exit 1
fi

run_with_limits "$exec_cmd"