import tkinter as tk


def test():
    print("Test")
class DesignWindow:
    def __init__(self, frame):
        self.frame = frame

        btn_project_info = tk.Button(self.frame, text="INPUT PROJECT INFORMATION", width=30, height=2, 
                                 bg="#333333", fg="white", command=test)
        btn_project_info.pack(pady=10)

        btn_riser_info = tk.Button(self.frame, text="INPUT RISER INFORMATION", width=30, height=2, 
                               bg="#333333", fg="white", command=test)
        btn_riser_info.pack(pady=10)

        btn_riser_capacities = tk.Button(self.frame, text="INPUT RISER CAPACITIES", width=30, height=2, 
                                     bg="#333333", fg="white",command=test)
        btn_riser_capacities.pack(pady=10)

        btn_riser_response = tk.Button(self.frame, text="INPUT RISER RESPONSE", width=30, height=2, 
                                   bg="#333333", fg="white",command=test)
        btn_riser_response.pack(pady=10)

        btn_bs_info = tk.Button(self.frame, text="INPUT BS INFORMATION", width=30, height=2, 
                            bg="#333333", fg="white",command=test)
        btn_bs_info.pack(pady=10)

    # Add OK and CANCEL buttons
        frame = tk.Frame(self.frame)
        frame.pack(pady=20)

        btn_ok = tk.Button(frame, text="OK", width=10, height=2, bg="#333333", fg="white")
        btn_ok.pack(side=tk.LEFT, padx=10)

def main():
    root = tk.Tk()
    app = DesignWindow(root)
    root.mainloop()

if __name__ == "__main__":
    main()