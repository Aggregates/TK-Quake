OPTS := /property:treatwarningsaserrors=false /property:langversion=6
STRICT_OPTS := /property:langversion=6
BUILD := xbuild
MSBUILD_PATH := '${ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe'
RM_OPTS := -r

ifeq ($(OS), Windows_NT)
	ifeq (,$(wildcard MSBUILD_PATH))
		BUILD = $(MSBUILD_PATH)
	endif
endif

all:
	$(BUILD) $(OPTS)

strict:
	$(BUILD) $(STRICT_OPTS)

clean:
	$(RM) $(RM_OPTS) bin
