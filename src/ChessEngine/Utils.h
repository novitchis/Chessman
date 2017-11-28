
#pragma once 

#define SafeCloseHandle(handle) if (handle) { CloseHandle( handle ); handle= NULL; }
#define BUFFER_SIZE 10 * 1024
#define sgn(x) ( ( x ) >= 0 ? 1 : -1 )

#include<vector>

template<typename T>
std::vector<T> 
	split(const T & str, const T & delimiters) {
		std::vector<T> v;
		T::size_type start = 0;
		auto pos = str.find_first_of(delimiters, start);
		while(pos != T::npos) {
			if(pos != start) // ignore empty tokens
				v.emplace_back(str, start, pos - start);
			start = pos + 1;
			pos = str.find_first_of(delimiters, start);
		}
		if(start < str.length()) // ignore trailing delimiter
			v.emplace_back(str, start, str.length() - start); // add what's left of the string
		return v;
}