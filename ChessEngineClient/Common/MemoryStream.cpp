#include "pch.h"
#include "MemoryStream.h"

#define BLOCK_SIZE		1024

MemoryStream::MemoryStream()
{
	m_strBuffer.reserve(BLOCK_SIZE);
	m_nStartIndex = 0;
}


MemoryStream::~MemoryStream()
{
}


bool MemoryStream::Write(const std::string& strData)
{
	Core::AutoLock MLock(&m_lock);
	Resize(strData.size());

	m_strBuffer.append(strData);
	m_writeEvt.raise();
	return true;
}



bool MemoryStream::ReadLine(std::string& strData)
{
	Core::AutoLock MLock(&m_lock);

	while (!GetAvailableByteCount())
	{
		m_lock.Unlock();
		m_writeEvt.wait();
		m_lock.Lock();
	}
	
	_ASSERTE(GetAvailableByteCount());
	auto nlPos = m_strBuffer.find('\n', m_nStartIndex);		
	if (nlPos == std::string::npos)
	{
		strData = m_strBuffer.substr(m_nStartIndex);
		m_nStartIndex = m_strBuffer.size();
	}
	else
	{
		strData = m_strBuffer.substr(m_nStartIndex, nlPos - m_nStartIndex);
		m_nStartIndex = nlPos + 1;
	}
	
	return true;
}


bool MemoryStream::ReadToEnd(std::string& strData)
{
	Core::AutoLock MLock(&m_lock);

	while (!GetAvailableByteCount())
	{
		m_lock.Unlock();
		m_writeEvt.wait();
		m_lock.Lock();
	}

	_ASSERTE(GetAvailableByteCount());

	strData = m_strBuffer.substr(m_nStartIndex);
	ResetCurrent();
	return true;
}


bool MemoryStream::Peek(int& nAvailableBytes)
{
	Core::AutoLock MLock(&m_lock);

	if (!GetAvailableByteCount())
	{
		m_lock.Unlock();
		m_writeEvt.wait();
		m_lock.Lock();
	}
	nAvailableBytes = GetAvailableByteCount();
	return true;
}


void MemoryStream::Resize(int nDesiredSize)
{
	auto bla = GetRemainingByteCount();
	if (GetRemainingByteCount() >= nDesiredSize) return;
	if (GetAvailableByteCount() == 0 && (nDesiredSize <= GetRemainingByteCount() + m_nStartIndex) ) {
		ResetCurrent();
		return;
	}
	m_strBuffer.reserve(2 * m_strBuffer.capacity());
}


int	MemoryStream::GetAvailableByteCount()
{
	return m_strBuffer.size() - m_nStartIndex;
}


int	MemoryStream::GetRemainingByteCount()
{
	return m_strBuffer.capacity() - m_nStartIndex;
}


void MemoryStream::ResetCurrent()
{
	m_nStartIndex = 0;
	m_strBuffer.resize(0);
}


//
//std::int_type MemoryStream::underflow()
//{
//	if (!GetAvailableByteCount()) return traits_type::eof;
//	return traits_type::to_int_type(m_strBuffer[m_nStartIndex]);
//}
//
//
//int_type MemoryStream::uflow()
//{
//
//}
//
//
//int_type MemoryStream::pbackfail(int_type ch)
//{
//
//}

