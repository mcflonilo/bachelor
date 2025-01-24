import tkinter as tk

def switch_frame(frame):
    frame.tkraise()
class ProjectInfoWindow:
    def __init__(self, root, prev_frame):
        self.root = root

        # Add title label
        lbl_title = tk.Label(self.root, text="PROJECT INFORMATION", font=("Arial", 14))
        lbl_title.pack(pady=10)

        # Project Name
        lbl_project_name = tk.Label(self.root, text="PROJECT NAME:", anchor="w")
        lbl_project_name.pack(fill="x", padx=20)
        self.entry_project_name = tk.Entry(self.root)
        self.entry_project_name.pack(fill="x", padx=20, pady=5)

        # Client
        lbl_client = tk.Label(self.root, text="CLIENT:", anchor="w")
        lbl_client.pack(fill="x", padx=20)
        self.entry_client = tk.Entry(self.root)
        self.entry_client.pack(fill="x", padx=20, pady=5)

        # Designer Name
        lbl_designer_name = tk.Label(self.root, text="DESIGNER NAME:", anchor="w")
        lbl_designer_name.pack(fill="x", padx=20)
        self.entry_designer_name = tk.Entry(self.root)
        self.entry_designer_name.pack(fill="x", padx=20, pady=5)

        # Add OK and CANCEL buttons
        frame_buttons = tk.Frame(self.root)
        frame_buttons.pack(pady=20)

        OK_buttons = tk.Button(root, text="OK", width=10, height=2, bg="#333333", fg="white", command=lambda: [self.get_data(), switch_frame(prev_frame)])
        OK_buttons.pack(pady=20)

    def get_data(self):
        """Return all data from the input fields."""
        data = {
            "project_name": self.entry_project_name.get(),
            "client": self.entry_client.get(),
            "designer_name": self.entry_designer_name.get()
        }
        return data

def main():
    root = tk.Tk()
    app = ProjectInfoWindow(root)
    root.mainloop()

if __name__ == "__main__":
    main()