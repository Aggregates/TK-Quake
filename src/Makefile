OPTS := /property:treatwarningsaserrors=false /property:langversion=6
STRICT_OPTS := /property:langversion=6
BUILD := xbuild
MSBUILD_PATH := '${ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe'
RM_OPTS := -r
ASSETS_PATH := assets

ifeq ($(OS), Windows_NT)
	ifeq (,$(wildcard MSBUILD_PATH))
		BUILD = $(MSBUILD_PATH)
	endif
endif

assets:
	mkdir -p $(ASSETS_PATH)
	curl -kL https://www.dropbox.com/s/452uec2yl87und9/nerfrevolver.obj?dl=1 -o '$(ASSETS_PATH)\nerfrevolver.obj'
	curl -kL https://www.dropbox.com/s/2v7vr6wc280zcrg/nerfrevolver.mtl?dl=1 -o '$(ASSETS_PATH)\nerfrevolver.mtl'
	curl -kL https://www.dropbox.com/s/3ex146i7z2ws4dr/nerfrevolverMapped.bmp?dl=1 -o '$(ASSETS_PATH)\nerfrevolverMapped.bmp'

	mkdir -p bin/Debug
	mkdir -p bin/Release
	cp $(ASSETS_PATH)/* bin/Debug
	cp $(ASSETS_PATH)/* bin/Release

all: assets
	$(BUILD) $(OPTS)

strict: assets
	$(BUILD) $(STRICT_OPTS)

clean:
	$(RM) $(RM_OPTS) bin
	$(RM) $(RM_OPTS) obj

clean-all: clean
	$(RM) $(RM_OPTS) $(ASSETS_PATH)
