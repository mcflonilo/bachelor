den komplette guien ligger i finalgui.py, den kjører ikke analyse per nå. hvis man vil gjøre det er det best å bruke start.py som ligger i oldstuff folderen

filen som inneholder multithreeading kode ligger i test.py som også er i oldstuff

for å bygge prosjektet:
pyinstaller --onefile --add-data "bsengine/bsengine.exe;bsengine" --add-data "bsengine/bsengine.lic;bsengine" --add-data "bsengine/._bsengine.lic;bsengine" --add-data "ultrabend_proposed_logo.ico;." --add-data "materials/material_database.json;materials" --icon=ultrabend_proposed_logo.ico arkitekturv2.py


Ting som må installeres for å kjøre programmet/ting for requirements.txt
- matplotlib
- scipy
- pandas
