#include "logger.h"

_INITIALIZE_EASYLOGGINGPP


void InitLogger(std::string fullpath)
    {
    el::Configurations defaultConf;
    defaultConf.setToDefault();
    //// Values are always std::string
    defaultConf.set(el::Level::Debug, el::ConfigurationType::Format, "%datetime %levshort [%func] [%loc] %msg");
    //// default logger uses default configurations
    //el::Loggers::reconfigureLogger("default", defaultConf);
    // To set GLOBAL configurations you may use
    //defaultConf.setGlobally(el::ConfigurationType::Format, "%date %msg");
    defaultConf.setGlobally(el::ConfigurationType::Filename, fullpath);
    el::Loggers::reconfigureLogger("default", defaultConf);
    LOG(INFO) << "Build: "__DATE__;
    }


//vararg MessageBox --------------------------------------------------------------------------------
void PrintBox(TCHAR *formstr,...)
    {
    TCHAR buffer[256];
    va_list vparam;
    va_start(vparam,formstr);
    _vstprintf(buffer,formstr,vparam);
    MessageBox(HWND_DESKTOP,buffer,_T(__FILE__),MB_OK|MB_TASKMODAL);
    va_end(vparam);
    }


//tests for a last error --------------------------------------------------------------------------
DWORD IsLastError(TCHAR* errmsg,bool mb)
    {
    DWORD retv=GetLastError();
    if(retv==ERROR_SUCCESS||retv==ERROR_IO_PENDING) return 0;
    LPVOID errbuf;
    FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM|FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,retv,MAKELANGID(LANG_NEUTRAL,SUBLANG_DEFAULT),(TCHAR*)&errbuf,0,NULL);
    LOG(DEBUG) << (TCHAR*)errbuf << _T("") << errmsg;
    if(mb)
        retv=MessageBox(HWND_DESKTOP,(TCHAR*)errbuf,errmsg,MB_ICONERROR|MB_ABORTRETRYIGNORE|MB_TASKMODAL|MB_TOPMOST);
    LocalFree(errbuf);
    if(retv==IDABORT) exit(retv);//return 1;
#ifdef _DEBUG
    if(retv==IDRETRY) DebugBreak();//return 1;
#endif
    return retv!=IDIGNORE;
    }

void LOGGER_UNIT_TESTS()
    {
    LOG(INFO) << "Starting test";
    LOG(DEBUG) << "Debug message";
    LOG(WARNING) << "Warning";
    LOG(ERROR) << "Error !";
    //LOG(FATAL) << "Fatal !!!";
    //LOG(TRACE) << "Trace";
    }