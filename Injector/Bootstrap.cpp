#include "stdafx.h"
#include "Bootstrap.h"

Bootstrapper * Bootstrapper::GetInstance()
{
	static auto* instance = new Bootstrapper();
	return instance;
}

bool Bootstrapper::Initialize()
{
	return Bootstrapper::HostClr();
}

bool Bootstrapper::HostClr()
{
	CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, reinterpret_cast<LPVOID*>(&this->pMetaHost));
	this->pMetaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&pRuntimeInfo));
	pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&this->pRuntimeHost));

	auto hr = this->pRuntimeHost->Start();

	return SUCCEEDED(hr);
}
