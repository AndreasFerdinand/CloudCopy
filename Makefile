linuxtarget = 'linux-x64'
windowstarget = 'win-x64'

.PHONY: all linux windows clean

all: linux windows

linux:
	mkdir -p releases
	dotnet publish src/ -c release -r $(linuxtarget) --self-contained
	#tar -cvzf CloudCopy-$(linuxtarget).tar.gz --directory=src/bin/release/netcoreapp3.0/$(linuxtarget)/ publish/
	cd src/bin/release/netcoreapp3.0/$(linuxtarget)/publish/ && tar -cvzf ../../../../../../releases/CloudCopy-$(linuxtarget).tar.gz *
	
windows:
	mkdir -p releases
	dotnet publish src/ -c release -r $(windowstarget) --self-contained
	cd src/bin/release/netcoreapp3.0/$(windowstarget)/publish/ && zip ../../../../../../releases/CloudCopy-$(windowstarget).zip *

clean:
	rm -rf src/bin/
	rm -rf releases/
