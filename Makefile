linuxtarget = linux-x64
windowstarget = win-x64
version := $(shell cat < 'Version')

.PHONY: all linux windows clean

all: lib-linux linux windows

lib-linux:
	echo "$(version)-$(linuxtarget)" > VersionName
	mkdir -p releases
	dotnet publish LibCloudCopy/src/ -c release -r $(linuxtarget) --self-contained
	cp LICENSE LibCloudCopy/src/bin/release/net5.0/$(linuxtarget)/publish/
	cd LibCloudCopy/src/bin/release/net5.0/$(linuxtarget)/publish/ && tar -cvzf ../../../../../../../releases/LibCloudCopy-$(version)-$(linuxtarget).tar.gz *

linux:
	echo "$(version)-$(linuxtarget)" > VersionName
	mkdir -p releases
	dotnet publish CloudCopyClient/src/ -c release -r $(linuxtarget) --self-contained
	cp LICENSE CloudCopyClient/src/bin/release/net5.0/$(linuxtarget)/publish/
	cd CloudCopyClient/src/bin/release/net5.0/$(linuxtarget)/publish/ && tar -cvzf ../../../../../../../releases/CloudCopy-$(version)-$(linuxtarget).tar.gz *
	
lib-windows:
	echo "$(version)-$(windowstarget)" > VersionName
	mkdir -p releases
	dotnet publish LibCloudCopy/src/ -c release -r $(windowstarget) --self-contained
	cp LICENSE LibCloudCopy/src/bin/release/net5.0/$(windowstarget)/publish/
	cd LibCloudCopy/src/bin/release/net5.0/$(windowstarget)/publish/ && tar -cvzf ../../../../../../../releases/LibCloudCopy-$(version)-$(windowstarget).tar.gz *

windows:
	echo "$(version)-$(windowstarget)" > VersionName
	mkdir -p releases
	dotnet publish CloudCopyClient/src/ -c release -r $(windowstarget) --self-contained
	cp LICENSE CloudCopyClient/src/bin/release/net5.0/$(windowstarget)/publish/
	cd CloudCopyClient/src/bin/release/net5.0/$(windowstarget)/publish/ && zip ../../../../../../../releases/CloudCopy-$(version)-$(windowstarget).zip *

clean:
	rm -rf src/bin/
	rm -rf releases/
