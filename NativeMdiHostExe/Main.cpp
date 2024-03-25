#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <objbase.h>

#if defined DEBUGWIN32
#include "..\Debug\WpfMdiClientWindowNE.h"
#pragma comment(lib, "..\\Debug\\WpfMdiClientWindowNE.lib")
#elif defined RELEASEWIN32
#include "..\Release\WpfMdiClientWindowNE.h"
#pragma comment(lib, "..\\Release\\WpfMdiClientWindowNE.lib")
#elif defined DEBUGX64
#include "..\x64\Debug\WpfMdiClientWindowNE.h"
#pragma comment(lib, "..\\x64\\Debug\\WpfMdiClientWindowNE.lib")
#elif defined RELEASEX64
#include "..\x64\Release\WpfMdiClientWindowNE.h"
#pragma comment(lib, "..\\x64\\Release\\WpfMdiClientWindowNE.lib")
#else
#error Unrecognized build configuration
#endif

HINSTANCE hInst;
HWND ghMDIClient;
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);

int APIENTRY wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{
	hInst = hInstance;

	// Register class
	{
		WNDCLASSEXW wcex{ 0 };
		wcex.cbSize = sizeof(WNDCLASSEX);
		wcex.style = CS_HREDRAW | CS_VREDRAW;
		wcex.lpfnWndProc = WndProc;
		wcex.hInstance = hInstance;
		wcex.lpszClassName = L"NativeMdiHostClass";
		RegisterClassExW(&wcex);
	}

	// Create
	HWND hWnd = CreateWindowW(L"NativeMdiHostClass", L"Example Host Window", WS_OVERLAPPEDWINDOW,
		CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);
	ShowWindow(hWnd, nCmdShow);
	UpdateWindow(hWnd);

	// Run
	::CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
	MSG msg;
	while (GetMessage(&msg, nullptr, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	::CoUninitialize();

	return (int)msg.wParam;
}

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case WM_CREATE:
	{
		// Add a menu to allow restore/minimize/maximize
		auto hMenu = CreateMenu();
		AppendMenu(hMenu, MF_STRING, NULL, L"File");
		SetMenu(hWnd, hMenu);

		// Add MDIClient
		CLIENTCREATESTRUCT mdiClientStruct{ nullptr, 1 };
		ghMDIClient = CreateWindow(L"MDIClient", NULL,
			WS_CHILD | WS_CLIPCHILDREN | WS_VISIBLE | WS_HSCROLL | WS_VSCROLL,
			0, 0, 0, 0,
			hWnd, NULL, hInst,
			&mdiClientStruct);

		// Create two child windows
		ShowNetcoreWpfMdiChild((intptr_t)ghMDIClient);
		ShowNetcoreWpfMdiChild((intptr_t)ghMDIClient);
	}
	break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefFrameProc(hWnd, ghMDIClient, message, wParam, lParam);
	}
	return 0;
}