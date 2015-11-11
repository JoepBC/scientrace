CONFIG=Debug
MONOC=mdtool build -c:$(CONFIG) -t:
MONOBUILD=$(MONOC)Build
MONOCLEAN=$(MONOC)Clean

all: scientrace-suite

scientrace-lib:
	$(MONOBUILD) scientrace-lib/scientrace-lib.csproj

scientrace-suite:
	$(MONOBUILD) scientrace-suite.sln

clean: 
	$(MONOCLEAN) scientrace-suite.sln
