linuxtarget = linux-x64
windowstarget = win-x64
version := $(shell cat < 'Version')

.PHONY: all linux windows clean

all: linux windows

linux:
	echo "$(version)-$(linuxtarget)" > VersionName
	mkdir -p releases
	dotnet publish src/ -c release -r $(linuxtarget) --self-contained
	cd src/bin/release/netcoreapp3.1/$(linuxtarget)/publish/ && tar -cvzf ../../../../../../releases/CloudCopy-$(version)-$(linuxtarget).tar.gz *
	
windows:
	echo "$(version)-$(windowstarget)" > VersionName
	mkdir -p releases
	dotnet publish src/ -c release -r $(windowstarget) --self-contained
	cd src/bin/release/netcoreapp3.1/$(windowstarget)/publish/ && zip ../../../../../../releases/CloudCopy-$(version)-$(windowstarget).zip *

clean:
	rm -rf src/bin/
	rm -rf releases/
