import os
import subprocess
import threading
import time

def is_file_locked(filepath):
    try:
        # Try to open the file exclusively
        fd = os.open(filepath, os.O_RDWR | os.O_EXCL)
        print("File is not locked.")
        os.close(fd)
        return False
    except OSError:
        return True

def run_exe():
    def run_case(case):
        try:
            # Get the current working directory
            current_directory = os.getcwd()
            # Run the executable file in the current directory
            process = subprocess.Popen(f".\\bsengine -b {case}", shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, cwd=current_directory)
            output, error = process.communicate()
            print("Output:", output)
        except subprocess.CalledProcessError as e:
            print("Error:", error)

    try:
        cases = open('testbsengine-cases.txt', 'r').readlines()
        cases = [case.strip() for case in cases]
        print(cases)

        license_file = 'bsengine.lic'  # Replace with the actual path to the license file

        max_threads = os.cpu_count()-1  # Get the number of CPU cores
        print(f"Max threads: {max_threads}")

        threads = []
        for i, case in enumerate(cases):
            while is_file_locked(license_file):
                print("License file is locked. Waiting for it to be released...")
                time.sleep(0.1)  # Wait for 100ms before checking again

            while threading.active_count() > max_threads:
                time.sleep(0.1)  # Wait for 100ms before checking again

            thread = threading.Thread(target=run_case, args=(case,))
            print(f"Running case {i + 1}...")
            threads.append(thread)
            thread.start()
            time.sleep(0.1)  # Wait for 100ms before checking again

        for thread in threads:
            thread.join()

    except Exception as e:
        print("Error:", str(e))

run_exe()