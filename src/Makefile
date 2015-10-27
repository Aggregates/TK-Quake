OPTS := /property:treatwarningsaserrors=false /property:langversion=6
STRICT_OPTS := /property:langversion=6
BUILD := xbuild
MSBUILD_PATH := '${ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe'
RM_OPTS := -r
ASSETS_PATH := assets
SKYBOX_PATH := '$(ASSETS_PATH)/skybox/space'

ifeq ($(OS), Windows_NT)
	ifeq (,$(wildcard MSBUILD_PATH))
		BUILD = $(MSBUILD_PATH)
	endif
endif

assets:
	mkdir -p $(ASSETS_PATH)
	mkdir -p $(SKYBOX_PATH)
	curl -kL https://www.dropbox.com/s/452uec2yl87und9/nerfrevolver.obj?dl=1 -o '$(ASSETS_PATH)\nerfrevolver.obj'
	curl -kL https://www.dropbox.com/s/2v7vr6wc280zcrg/nerfrevolver.mtl?dl=1 -o '$(ASSETS_PATH)\nerfrevolver.mtl'
	curl -kL https://www.dropbox.com/s/3ex146i7z2ws4dr/nerfrevolverMapped.bmp?dl=1 -o '$(ASSETS_PATH)\nerfrevolverMapped.bmp'
	curl -kL https://www.dropbox.com/s/rcyqd5cvvjvub8f/s-back.bmp?dl=1 -o '$(SKYBOX_PATH)\back.bmp'
	curl -kL https://www.dropbox.com/s/w2vg00u4rjsnhq3/s-front.bmp?dl=1 -o '$(SKYBOX_PATH)\front.bmp'
	curl -kL https://www.dropbox.com/s/lrz4rj2z9yskg3m/s-left.bmp?dl=1 -o '$(SKYBOX_PATH)\left.bmp'
	curl -kL https://www.dropbox.com/s/gq90ctamnaeimqb/s-right.bmp?dl=1 -o '$(SKYBOX_PATH)\right.bmp'
	curl -kL https://www.dropbox.com/s/l1o93c402sb6kii/s-top.bmp?dl=1 -o '$(SKYBOX_PATH)\top.bmp'

	curl -kL https://www.dropbox.com/s/rw4l3cfozt5o9ae/FireParticle1.png?dl=0 -o '$(ASSETS_PATH)\FireParticle1.png'
	curl -kL https://www.dropbox.com/s/r9nz2mmhdaer1h7/FireParticle.png?dl=0 -o '$(ASSETS_PATH)\FireParticle.png'
	curl -kL https://www.dropbox.com/s/s74sc10dtenvkg2/floor_s.jpg?dl=0 -o '$(ASSETS_PATH)\floor_s.jpg'
	curl -kL https://www.dropbox.com/s/61un0mszvbiobu3/Floor_t.png?dl=0 -o '$(ASSETS_PATH)\Floor_t.png'
	curl -kL https://www.dropbox.com/s/j4qyhh6u3u1ncqf/floor_w.jpg?dl=0 -o '$(ASSETS_PATH)\floor_w.jpg'
	curl -kL https://www.dropbox.com/s/jcy9aznbsr9p4jj/Smoke.png?dl=0 -o '$(ASSETS_PATH)\Smoke.png'


	mkdir -p bin/Debug
	mkdir -p bin/Release
	cp -R $(ASSETS_PATH)/* bin/Debug
	cp -R $(ASSETS_PATH)/* bin/Release

all: assets
	$(BUILD) $(OPTS)

strict: assets
	$(BUILD) $(STRICT_OPTS)

clean:
	$(RM) $(RM_OPTS) bin
	$(RM) $(RM_OPTS) obj

clean-all: clean
	$(RM) $(RM_OPTS) $(ASSETS_PATH)
