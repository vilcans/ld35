PROJECT=EuclidGeomorph
VERSION=0.2
FILENAME=$(PROJECT)-$(VERSION)
RELEASE_DIR=Build/release

DEPLOY_PATH=filur:/opt/public/$(PROJECT)

release: release-win release-mac release-linux release-web

release-web:
	cp -r Build/web/* Build/release/

release-win: $(RELEASE_DIR)/$(FILENAME)-win32.zip

$(RELEASE_DIR)/$(FILENAME)-win32.zip:
	mkdir -p Build/release
	rm -rf Build/ziptemp
	mkdir -p Build/ziptemp
	cp -r Build/win32 Build/ziptemp/$(FILENAME)
	bash -c 'cd Build/ziptemp && zip -x .DS_Store -x '*.pdb' -r ../release/$(FILENAME)-win32.zip $(FILENAME)'

release-mac: $(RELEASE_DIR)/$(FILENAME).dmg

$(RELEASE_DIR)/$(FILENAME).dmg:
	mkdir -p Build/release
	hdiutil create $@ -volname "$(PROJECT)" -srcfolder Build/mac

release-linux: $(RELEASE_DIR)/$(FILENAME)-linux.tar.gz

$(RELEASE_DIR)/$(FILENAME)-linux.tar.gz:
	mkdir -p Build/release
	rm -rf Build/ziptemp
	mkdir -p Build/ziptemp
	cp -r Build/linux Build/ziptemp/$(FILENAME)
	tar -czf $@ --exclude '.DS_Store' -C Build/ziptemp $(FILENAME)

release-android:
	cp Build/android/$(PROJECT).apk Build/release/$(FILENAME).apk

clean:
	rm -rf Build/release
	rm -rf Build/ziptemp

deploy:
	rsync -avz Build/release/* $(DEPLOY_PATH)/

%.gif:%.mov
	ffmpeg -i $< -vf scale=320:-1 -pix_fmt rgb24 -r 15 -f gif - | gifsicle --optimize=3 --delay=9 >$@

.PHONY: release
