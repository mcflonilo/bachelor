import tkinter as tk

def switch_frame(frame):
    frame.tkraise()

class ProjectInfoWindow:
    def __init__(self, root, prev_frame):
        self.root = root

        # Add title label
        lbl_title = tk.Label(self.root, text="PROJECT INFORMATION", font=("Arial", 14))
        lbl_title.grid(row=0, column=0, columnspan=2, pady=10)

        # Project Name
        lbl_project_name = tk.Label(self.root, text="PROJECT NAME:", anchor="w")
        lbl_project_name.grid(row=1, column=0, sticky="w", padx=20, pady=5)
        self.entry_project_name = tk.Entry(self.root)
        self.entry_project_name.grid(row=1, column=1, padx=20, pady=5)

        # Client
        lbl_client = tk.Label(self.root, text="CLIENT:", anchor="w")
        lbl_client.grid(row=2, column=0, sticky="w", padx=20, pady=5)
        self.entry_client = tk.Entry(self.root)
        self.entry_client.grid(row=2, column=1, padx=20, pady=5)

        # Designer Name
        lbl_designer_name = tk.Label(self.root, text="DESIGNER NAME:", anchor="w")
        lbl_designer_name.grid(row=3, column=0, sticky="w", padx=20, pady=5)
        self.entry_designer_name = tk.Entry(self.root)
        self.entry_designer_name.grid(row=3, column=1, padx=20, pady=5)

        # Add OK and CANCEL buttons
        frame_buttons = tk.Frame(self.root)
        frame_buttons.grid(row=4, column=0, columnspan=2, pady=20)
        btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white", command=lambda: [self.get_data(), switch_frame(prev_frame)])
        btn_ok.grid(row=0, column=0, padx=10)

    def get_data(self):
        """Return all data from the input fields."""
        data = {
            "project_name": self.entry_project_name.get(),
            "client": self.entry_client.get(),
            "designer_name": self.entry_designer_name.get()
        }
        return data
    
    def set_data(self, data):
        """Set data to the input fields."""
        self.entry_project_name.delete(0, tk.END)
        self.entry_project_name.insert(0, data.get("project_name", ""))
        self.entry_client.delete(0, tk.END)
        self.entry_client.insert(0, data.get("client", ""))
        self.entry_designer_name.delete(0, tk.END)
        self.entry_designer_name.insert(0, data.get("designer_name", ""))


def main():
    root = tk.Tk()
    prev_frame = tk.Frame(root)
    prev_frame.grid(row=0, column=0, sticky="nsew")
    app = ProjectInfoWindow(root, prev_frame)
    root.mainloop()

if __name__ == "__main__":
    main()