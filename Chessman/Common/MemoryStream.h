#pragma once
#include <vector>
#include "Event.h"
#include "AutoLock.h"
#include <streambuf>

class MemoryStream
{
public:
	MemoryStream();
	~MemoryStream();

	bool	Write(const std::string& strData);
	bool	ReadLine(std::string& strData);
	bool	ReadToEnd(std::string& strData);
	bool	Peek(int& nAvailableBytes);

private:
	void	Resize(int nDesiredSize);
	int		GetAvailableByteCount();
	int		GetRemainingByteCount();
	void	ResetCurrent();
	
private:
	std::string				m_strBuffer;
	int						m_nStartIndex;
	int						m_nSize;
	Core::Event				m_writeEvt;
	Core::CriticalSection	m_lock;
};


// TODO
//class EngineStream: public std::streambuf
//{
//public:
//	EngineStream(const char *data, unsigned int len);
//
//private:
//	int_type underflow();
//	int_type uflow();
//	int_type pbackfail(int_type ch);
//	std::streamsize showmanyc();
//};