import os
import glob

def delete_fea_files(folder_path):
    """
    Deletes all files in the specified folder that end with 'FEA'.
    
    :param folder_path: Path to the folder where files will be deleted.
    """
    # Ensure the folder exists
    if not os.path.exists(folder_path):
        print(f"Folder not found: {folder_path}")
        return
    
    # Get all files ending with 'FEA'
    fea_files = glob.glob(os.path.join(folder_path, "*FEA.log"))
    
    # Check if there are files to delete
    if not fea_files:
        print("No files ending with 'FEA' found in the folder.")
        return

    # Delete each file
    for file_path in fea_files:
        try:
            os.remove(file_path)
            print(f"Deleted: {file_path}")
        except Exception as e:
            print(f"Error deleting {file_path}: {e}")

# Specify the folder path here
folder_path = "C:/Users/lmoph/Desktop/github/bachelor/project/case_files"

# Call the function
delete_fea_files(folder_path)
