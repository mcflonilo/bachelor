class DataStructure:
    def __init__(self, data):
        self.project_info = data.get("project_info", {})
        self.riser_info = data.get("riser_info", {})
        self.riser_capacities = data.get("riser_capacities", {})
        self.riser_response = data.get("riser_response", {})
        self.bs_dimension = data.get("bs_dimension", {})
        self.bs_material = data.get("bs_material", {})

    def validate(self):
        errors = []

        # Validate integer fields
        int_fields = [
            ("riser_info", "Riser Length"),
            ("bs_dimension", "Input root length:"),
            ("bs_dimension", "Input tip length:"),
            ("bs_dimension", "Input MIN root OD:"),
            ("bs_dimension", "Input MAX root OD:"),
            ("bs_dimension", "Input MIN overall length:"),
            ("bs_dimension", "Input MAX overall length:"),
            ("bs_dimension", "Increment Width:"),
            ("bs_dimension", "Increment Length:")
        ]
        
        for category, key in int_fields:
            if key in self.__dict__[category]:
                try:
                    self.__dict__[category][key] = int(self.__dict__[category][key])
                except ValueError:
                    errors.append(f"{key} should be an integer.")

        # Validate float fields
        float_fields = [
            ("riser_capacities", "normal"),
            ("riser_capacities", "abnormal"),
            ("riser_response", "normal"),
            ("riser_response", "abnormal")
        ]

        for category, key in float_fields:
            if key in self.__dict__[category]:
                try:
                    self.__dict__[category][key] = [
                        [float(value) for value in entry] for entry in self.__dict__[category][key]
                    ]
                except ValueError:
                    errors.append(f"All values in {key} should be floats.")

        # Ensure all fields are filled
        for category, sub_dict in self.__dict__.items():
            if isinstance(sub_dict, dict):
                for key, value in sub_dict.items():
                    if value in [None, "", []]:  # Check for empty values
                        errors.append(f"{key} in {category} is empty.")

        return errors

# Usage example:
data = {  # Replace this with your actual data
    "project_info": {"project_name": "awd", "client": "ddawd", "designer_name": "awd"},
    "riser_info": {"Riser Identification": "awd", "Outer Diameter": "wda", "Riser Length": "10"},
    "riser_capacities": {"normal": [[0.08197, 0.0], [0.07752, 87.0]]},
    "riser_response": {"normal": [[4.0, 18.0, 23.0], [550.0, 650.0, 750.0]]},
    "bs_dimension": {"Input root length:": "12", "Input tip length:": "14"},
    "bs_material": {"Section 1": {"material_characteristics": "Silicon", "elastic_modules": "1"}}
}

riser_system = DataStructure(data)
errors = riser_system.validate()

if errors:
    print("Validation errors found:")
    for error in errors:
        print(f"- {error}")
else:
    print("All inputs are valid!")
