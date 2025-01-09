import re

def process_cases(case_list_file, summary_file_name):
    def extract_key_results(case_file, summary_file):
        try:
            with open(case_file, 'r') as res_file:
                res_lines = res_file.readlines()

            keyres1 = None
            keyres2 = None

        # Search for key results in the log file
            for i, line in enumerate(res_lines):
                if re.search(r'Maximum BS curvature', line):
                    keyres1 = line.strip().split(':')[-1].strip()
                if re.search(r'Maximum curvature', line):
                    keyres2 = line.strip().split(':')[-1].strip()

            if keyres1 and keyres2:
                casename = case_file.rstrip('.log')
                keyres = f"{casename} {keyres1} {keyres2}\n"
                summary_file.write(keyres)
            else:
                print(f"Key results not found in {case_file}")

        except FileNotFoundError:
            print(f"File not found: {case_file}")
        except Exception as e:
            print(f"Error processing {case_file}: {e}")
    try:
        with open(case_list_file, 'r') as f:
            cases = f.readlines()

        with open(summary_file_name, 'w') as summary:
            summary.write("Load Case     Maximum BS curvature     Maximum curvature\n")

            for case in cases:
                case_file = case.strip().replace('.inp', '.log')
                extract_key_results(case_file, summary)

    except FileNotFoundError:
        print(f"File not found: {case_list_file}")
    except Exception as e:
        print(f"Error processing {case_list_file}: {e}")

process_cases('bsengine-cases.txt', '40-results1.txt')