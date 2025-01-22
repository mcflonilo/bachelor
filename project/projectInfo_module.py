import tkinter as tk


def test():
    print("Test")

def switch_frame(frame):
    frame.tkraise()

class ProjectInfoWindow:
    def __init__(self, root):
        self.root = root

        # Add title label
        lbl_title = tk.Label(self.root, text="PROJECT INFORMATION", font=("Arial", 14))
        lbl_title.pack(pady=10)

        # Project Name
        lbl_project_name = tk.Label(self.root, text="PROJECT NAME:", anchor="w")
        lbl_project_name.pack(fill="x", padx=20)
        entry_project_name = tk.Entry(self.root)
        entry_project_name.pack(fill="x", padx=20, pady=5)

        # Client
        lbl_client = tk.Label(self.root, text="CLIENT:", anchor="w")
        lbl_client.pack(fill="x", padx=20)
        entry_client = tk.Entry(self.root)
        entry_client.pack(fill="x", padx=20, pady=5)

        # Designer Name
        lbl_designer_name = tk.Label(self.root, text="DESIGNER NAME:", anchor="w")
        lbl_designer_name.pack(fill="x", padx=20)
        entry_designer_name = tk.Entry(self.root)
        entry_designer_name.pack(fill="x", padx=20, pady=5)

        # Add OK and CANCEL buttons
        frame_buttons = tk.Frame(self.root)
        frame_buttons.pack(pady=20)

        btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white")
        btn_ok.pack(side=tk.LEFT, padx=10)

def main():
    root = tk.Tk()
    app = ProjectInfoWindow(root)
    root.mainloop()

if __name__ == "__main__":
    main()