#pragma once
#include <metahost.h>
#include <mscoree.h>
#include <stdio.h>
#include <direct.h>
#pragma comment(lib, "mscoree.lib")


class Bootstrapper
{
private:
	ICLRMetaHost* pMetaHost = nullptr;
	ICLRRuntimeHost* pRuntimeHost = nullptr;
	ICLRRuntimeInfo* pRuntimeInfo = nullptr;
public:
	static Bootstrapper* GetInstance();

	bool Initialize();
	bool HostClr();
};