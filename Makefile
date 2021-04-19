linuxtarget = linux-x64
windowstarget = win-x64
version := $(shell cat < 'Version')
dotnetrelease = net5.0

.PHONY: all linux windows clean

all: lib-linux linux lib-windows windows

lib-linux:
	echo "$(version)-$(linuxtarget)" > VersionName
	mkdir -p releases
	dotnet publish LibCloudCopy/src/ -c release -r $(linuxtarget) --self-contained  --framework $(dotnetrelease)
	cp LICENSE LibCloudCopy/src/bin/release/$(dotnetrelease)/$(linuxtarget)/publish/
	cd LibCloudCopy/src/bin/release/$(dotnetrelease)/$(linuxtarget)/publish/ && tar -cvzf ../../../../../../../releases/LibCloudCopy-$(version)-$(linuxtarget).tar.gz *

linux:
	echo "$(version)-$(linuxtarget)" > VersionName
	mkdir -p releases
	dotnet publish CloudCopyClient/src/ -c release -r $(linuxtarget) --self-contained --framework $(dotnetrelease)
	cp LICENSE CloudCopyClient/src/bin/release/$(dotnetrelease)/$(linuxtarget)/publish/
	cd CloudCopyClient/src/bin/release/$(dotnetrelease)/$(linuxtarget)/publish/ && tar -cvzf ../../../../../../../releases/CloudCopy-$(version)-$(linuxtarget).tar.gz *
	
lib-windows:
	echo "$(version)-$(windowstarget)" > VersionName
	mkdir -p releases
	dotnet publish LibCloudCopy/src/ -c release -r $(windowstarget) --framework $(dotnetrelease) --no-self-contained
	cp LICENSE LibCloudCopy/src/bin/release/$(dotnetrelease)/$(windowstarget)/publish/
	cd LibCloudCopy/src/bin/release/$(dotnetrelease)/$(windowstarget)/publish/ && zip ../../../../../../../releases/LibCloudCopy-$(version)-$(windowstarget).zip *

windows:
	echo "$(version)-$(windowstarget)" > VersionName
	mkdir -p releases
	dotnet publish CloudCopyClient/src/ -c release -r $(windowstarget) --self-contained  --framework $(dotnetrelease)
	cp LICENSE CloudCopyClient/src/bin/release/$(dotnetrelease)/$(windowstarget)/publish/
	cd CloudCopyClient/src/bin/release/$(dotnetrelease)/$(windowstarget)/publish/ && zip ../../../../../../../releases/CloudCopy-$(version)-$(windowstarget).zip *

clean:
	rm -rf CloudCopyClient/src/bin/
	rm -rf LibCloudCopy/src/bin/
	rm -rf releases/
