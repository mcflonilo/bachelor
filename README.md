spørsmål: er det meningen at de eneste parameterne for bøystiver skal være lengde og bredde?
det er mange hardkodede variabler i input filen, skal det være sånn eller skal bruker kunne endree på disse.
er tykkelsen på alle bøystivere lik? fra det jeg skjønner vil alltid tupp tykkelse være 40mm større enn umbilical OD
er de hule?
for hver iterasjon vil kun cone length(CL) og/eller outer diameter endre seg
hvis de eneste variablenme skal være min lengde- max lengde og min bredde til max bredde. hvordan påvirker dette de andre parameterene. feks tip lengtrh, segment length
hvordan regner man ut max curvature for hver case.



hvordan best designe optimalisator ideer:

sjekke siste og første fil absolutt først. dette finner om alle stivere vil være gyldige eller ugyldige

potensielt kjøre grove hopp til man finner en som fungerer så zoome inn på område

hvis alle resultatfiler vil være grønne kun nederst til høyre vil man potensielt kunne starte i midten og ekskludere mange resultater basert på det. så gjøre et nytt hopp til nytt område og ekskludere mer
