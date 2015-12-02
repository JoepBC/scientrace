CONFIG=Debug
MONOC=mdtool -v build -c:$(CONFIG) -t:
MONOBUILD=$(MONOC)Build
MONOCLEAN=$(MONOC)Clean

all: scientrace-suite

xml-only:
	$(MONOBUILD) scientrace-lib/scientrace-lib.sln
	$(MONOBUILD) scientrace-xml/scientrace-xml.sln

scientrace-suite:
	$(MONOBUILD) scientrace-suite.sln

clean: 
	$(MONOCLEAN) scientrace-lib/scientrace-lib.sln
	$(MONOCLEAN) scientrace-xml/scientrace-xml.sln
	$(MONOCLEAN) scientrace-suite.sln
