linuxtarget = linux-x64
windowstarget = win-x64
dotnetrelease = net5.0
version = 0.5.1

.PHONY: all linux windows clean

all: lib-linux linux lib-windows windows

lib-linux:
	mkdir -p releases
	dotnet publish LibCloudCopy/src/ -c release -r $(linuxtarget) --self-contained  --framework $(dotnetrelease) /p:Version=$(version)-$(linuxtarget)
	cp LICENSE LibCloudCopy/src/bin/release/$(dotnetrelease)/$(linuxtarget)/publish/
	cd LibCloudCopy/src/bin/release/$(dotnetrelease)/$(linuxtarget)/publish/ && tar -cvzf ../../../../../../../releases/LibCloudCopy-$(versionprefix)-$(linuxtarget).tar.gz *

linux:
	mkdir -p releases
	dotnet publish CloudCopyClient/src/ -c release -r $(linuxtarget) --self-contained --framework $(dotnetrelease) /p:Version=$(version)-$(linuxtarget)
	cp LICENSE CloudCopyClient/src/bin/release/$(dotnetrelease)/$(linuxtarget)/publish/
	cd CloudCopyClient/src/bin/release/$(dotnetrelease)/$(linuxtarget)/publish/ && tar -cvzf ../../../../../../../releases/CloudCopy-$(versionprefix)-$(linuxtarget).tar.gz *
	
lib-windows:
	mkdir -p releases
	dotnet publish LibCloudCopy/src/ -c release -r $(windowstarget) --framework $(dotnetrelease) --no-self-contained /p:Version=$(version)-$(linuxtarget)
	cp LICENSE LibCloudCopy/src/bin/release/$(dotnetrelease)/$(windowstarget)/publish/
	cd LibCloudCopy/src/bin/release/$(dotnetrelease)/$(windowstarget)/publish/ && zip ../../../../../../../releases/LibCloudCopy-$(versionprefix)-$(windowstarget).zip *

windows:
	mkdir -p releases
	dotnet publish CloudCopyClient/src/ -c release -r $(windowstarget) --self-contained  --framework $(dotnetrelease) /p:Version=$(version)-$(linuxtarget)
	cp LICENSE CloudCopyClient/src/bin/release/$(dotnetrelease)/$(windowstarget)/publish/
	cd CloudCopyClient/src/bin/release/$(dotnetrelease)/$(windowstarget)/publish/ && zip ../../../../../../../releases/CloudCopy-$(versionprefix)-$(windowstarget).zip *

clean:
	rm -rf CloudCopyClient/src/bin/
	rm -rf LibCloudCopy/src/bin/
	rm -rf releases/
