import os

def delete_unpaired_files(directory):
    file_groups = {}
    fea_log_files = set()

    # Step 1: Scan directory and group files by their name (without extension)
    for filename in os.listdir(directory):
        if os.path.isfile(os.path.join(directory, filename)):
            name, ext = os.path.splitext(filename)
            
            # Special handling for "_FEA.log"
            if name.endswith("_FEA") and ext == ".log":
                base_name = name[:-4]  # Remove "_FEA" from the name
                fea_log_files.add(name)
            
            if name not in file_groups:
                file_groups[name] = set()
            file_groups[name].add(ext)

    # Step 2: Identify files that have no counterpart with a different extension and are not part of _FEA.log group
    for name, extensions in file_groups.items():
        if len(extensions) == 1 and name not in fea_log_files:  # Only one extension, and no _FEA.log match
            file_to_delete = f"{name}{list(extensions)[0]}"
            file_path = os.path.join(directory, file_to_delete)
            print(f"Deleting: {file_path}")  # Debugging message
            os.remove(file_path)  # Uncomment to actually delete the files

# Example usage
directory_path = "C:\\Users\\lmoph\\OneDrive\\Skrivebord\\git_repos\\bachelor\\project\\case_files"  # Change this to your target directory
delete_unpaired_files(directory_path)
