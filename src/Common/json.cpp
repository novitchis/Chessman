#include "pch.h"
#include <windows.h>
#include "json.h"
#include <stdlib.h>
#include <string>
#include <algorithm>
#include <cstdlib>
#include <cstdio>
#include <climits>
#include <string.h>
#include <functional>
#include <cctype>
#include <stack>


#ifndef WIN32
#define _stricmp strcasecmp
#endif

#ifdef _MSC_VER
#define snprintf sprintf_s
#endif

using namespace json;

namespace json
{
	enum StackDepthType
	{
		InObject,
		InArray
	};
}


// replacement for !std::isspace, because that one throws an assert for UTF8 chracters
int utf8_not_space(int c)
    {
    switch(c)
        {
        case 0x20: //' '
            return false;
        case 0x0c: //'\f'
            return false;
        case 0x0a: //'\n'
            return false;
        case 0x0d: //'\r'
            return false;
        case 0x09: //'\t'
            return false;
        case 0x0b: //'\v' vertical tab
            return false;
        }
    return true;
    }


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Helper functions
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
static std::string Trim(const std::string& str)
{
	std::string s = str;
	
	try  //only works on ASCII
	{
    // remove white space in front
    s.erase(s.begin(), std::find_if(s.begin(), s.end(), utf8_not_space));

    // remove trailing white space
    s.erase(std::find_if(s.rbegin(), s.rend(), utf8_not_space).base(), s.end());
	}
	catch (...)
	{
	}
	
	return s;
}

// Finds the position of the first " character that is NOT preceeded immediately by a \ character.
// In JSON, \" is valid and has a different meaning than the escaped " character.
static size_t GetQuotePos(const std::string& str, size_t start_pos = 0)
{
	bool found_slash = false;
	for (size_t i = start_pos; i < str.length(); i++)
	{
		char c = str[i];
		if ((c == '\\') && !found_slash)
		{
			found_slash = true;
			continue;
		}
		else if ((c == '\"') && !found_slash)
			return i;
		
		found_slash = false;
	}
	
	return std::string::npos;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Value::Value(const Value& v) : mValueType(v.mValueType)
{
	switch (mValueType)
	{
		case StringVal		: mStringVal = v.mStringVal; break;
		case IntVal			: mIntVal = v.mIntVal; mFloatVal = (float)v.mIntVal; mDoubleVal = (double)v.mIntVal; break;
		case FloatVal		: mFloatVal = v.mFloatVal; mIntVal = (int)v.mFloatVal; mDoubleVal = (double)v.mDoubleVal; break;
		case DoubleVal		: mDoubleVal = v.mDoubleVal; mIntVal = (int)v.mDoubleVal; mFloatVal = (float)v.mDoubleVal; break;
		case BoolVal		: mBoolVal = v.mBoolVal; break;
		case ObjectVal		: mObjectVal = v.mObjectVal; break;
		case ArrayVal		: mArrayVal = v.mArrayVal; break;
		default				: break;
	}
}

Value& Value::operator =(const Value& v)
{
	if (&v == this)
		return *this;

	mValueType = v.mValueType;

	switch (mValueType)
	{
		case StringVal		: mStringVal = v.mStringVal; break;
		case IntVal			: mIntVal = v.mIntVal; mFloatVal = (float)v.mIntVal; mDoubleVal = (double)v.mIntVal; break;
		case FloatVal		: mFloatVal = v.mFloatVal; mIntVal = (int)v.mFloatVal; mDoubleVal = (double)v.mDoubleVal; break;
		case DoubleVal		: mDoubleVal = v.mDoubleVal; mIntVal = (int)v.mDoubleVal; mFloatVal = (float)v.mDoubleVal; break;
		case BoolVal		: mBoolVal = v.mBoolVal; break;
		case ObjectVal		: mObjectVal = v.mObjectVal; break;
		case ArrayVal		: mArrayVal = v.mArrayVal; break;
		default				: break;
	}

	return *this;
}

Value& Value::operator [](size_t idx)
{
	if (mValueType != ArrayVal)
		throw std::runtime_error("json mValueType==ArrayVal required");

	return mArrayVal[idx];
}

const Value& Value::operator [](size_t idx) const
{
	if (mValueType != ArrayVal)
		throw std::runtime_error("json mValueType==ArrayVal required");

	return mArrayVal[idx];
}

Value& Value::operator [](const std::string& key)
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal[key];
}

Value& Value::operator [](const char* key)
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal[key];
}

const Value& Value::operator [](const char* key) const
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal[key];
}

const Value& Value::operator [](const std::string& key) const
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal[key];
}

void Value::Clear()
{
	mValueType = NULLVal;
}

size_t Value::size() const
{
	if ((mValueType != ObjectVal) && (mValueType != ArrayVal))
		return 1;

	return mValueType == ObjectVal ? mObjectVal.size() : mArrayVal.size();
}

bool Value::HasKey(const std::string &key) const
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal.HasKey(key);
}

int Value::HasKeys(const std::vector<std::string> &keys) const
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal.HasKeys(keys);
}

int Value::HasKeys(const char **keys, int key_count) const
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal.HasKeys(keys, key_count);
}

int Value::ToInt() const		
{
	if (!IsNumeric())
		throw std::runtime_error("json mValueType==IsNumeric() required");

	return mIntVal;
}

float Value::ToFloat() const		
{
	if (!IsNumeric())
		throw std::runtime_error("json mValueType==IsNumeric() required");
	
	return mFloatVal;
}

double Value::ToDouble() const	
{
	if (!IsNumeric())
		throw std::runtime_error("json mValueType==IsNumeric() required");

	return mDoubleVal;
}

bool Value::ToBool() const		
{
	if (mValueType != BoolVal)
		throw std::runtime_error("json mValueType==BoolVal required");
	
	return mBoolVal;
}

const std::string& Value::ToString() const	
{
	if (mValueType != StringVal)
		throw std::runtime_error("json mValueType==StringVal required");
	
	return mStringVal;
}

Object Value::ToObject() const	
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal;
}

Array Value::ToArray() const		
{
	if (mValueType != ArrayVal)
		throw std::runtime_error("json mValueType==ArrayVal required");

	return mArrayVal;
}

Value::operator int() const
{ 
	if (!IsNumeric())
		throw std::runtime_error("json mValueType==IsNumeric() required");

	return mIntVal;
}

Value::operator float() const 			
{	
	if (!IsNumeric())
		throw std::runtime_error("json mValueType==IsNumeric() required");

	return mFloatVal;
}

Value::operator double() const
{
	if (!IsNumeric())
		throw std::runtime_error("json mValueType==IsNumeric() required");

	return mDoubleVal;
}

Value::operator bool() const 			
{
	if (mValueType != BoolVal)
		throw std::runtime_error("json mValueType==BoolVal required");

	return mBoolVal;
}

Value::operator std::string() const 	
{
	if (mValueType != StringVal)
		throw std::runtime_error("json mValueType==StringVal required");

	return mStringVal;
}

Value::operator Object() const 		
{
	if (mValueType != ObjectVal)
		throw std::runtime_error("json mValueType==ObjectVal required");

	return mObjectVal;
}

Value::operator Array() const 			
{
	if (mValueType != ArrayVal)
		throw std::runtime_error("json mValueType==ArrayVal required");

	return mArrayVal;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Array::Array()
{
}

Array::Array(const Array& a) : mValues(a.mValues)
{
}

Array& Array::operator =(const Array& a)
{
	if (&a == this)
		return *this;

	Clear();
	mValues = a.mValues;

	return *this;
}

Value& Array::operator [](size_t i)
{
	return mValues[i];
}

const Value& Array::operator [](size_t i) const
{
	return mValues[i];
}


Array::ValueVector::const_iterator Array::begin() const
{
	return mValues.begin();
}

Array::ValueVector::const_iterator Array::end() const
{
	return mValues.end();
}

Array::ValueVector::iterator Array::begin()
{
	return mValues.begin();
}

Array::ValueVector::iterator Array::end()
{
	return mValues.end();
}

void Array::push_back(const Value& v)
{
	mValues.push_back(v);
}

void Array::insert(size_t index, const Value& v)
{
	mValues.insert(mValues.begin() + index, v);
}

size_t Array::size() const
{
	return mValues.size();
}

void Array::Clear()
{
	mValues.clear();
}

Array::ValueVector::iterator Array::find(const Value& v)
{
	return std::find(mValues.begin(), mValues.end(), v);
}

Array::ValueVector::const_iterator Array::find(const Value& v) const
{
	return std::find(mValues.begin(), mValues.end(), v);
}

bool Array::HasValue(const Value& v) const
{
	return find(v) != end();
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Object::Object()
{
}

Object::Object(const Object& obj) : mValues(obj.mValues)
{

}

Object& Object::operator =(const Object& obj)
{
	if (&obj == this)
		return *this;

	Clear();
	mValues = obj.mValues;

	return *this;
}

Value& Object::operator [](const std::string& key)
{
	return mValues[key];
}

const Value& Object::operator [](const std::string& key) const
{
	ValueMap::const_iterator it = mValues.find(key);
	return it->second;
}

Value& Object::operator [](const char* key)
{
	return mValues[key];
}

const Value& Object::operator [](const char* key) const
{
	ValueMap::const_iterator it = mValues.find(key);
	return it->second;
}

Object::ValueMap::const_iterator Object::begin() const
{
	return mValues.begin();
}

Object::ValueMap::const_iterator Object::end() const
{
	return mValues.end();
}

Object::ValueMap::iterator Object::begin()
{
	return mValues.begin();
}

Object::ValueMap::iterator Object::end()
{
	return mValues.end();
}

Object::ValueMap::iterator Object::find(const std::string& key)
{
	return mValues.find(key);
}

Object::ValueMap::const_iterator Object::find(const std::string& key) const
{
	return mValues.find(key);
}

bool Object::HasKey(const std::string& key) const
{
	return find(key) != end();
}

int Object::HasKeys(const std::vector<std::string>& keys) const
{
	for (size_t i = 0; i < keys.size(); i++)
	{
		if (!HasKey(keys[i]))
			return (int)i;
	}
	
	return -1;
}

int Object::HasKeys(const char** keys, int key_count) const
{
	for (int i = 0; i < key_count; i++)
		if (!HasKey(keys[i]))
			return i;
	
	return -1;
}

void Object::Clear()
{
	mValues.clear();
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
std::string SerializeArray(const Array& a);

std::string SerializeValue(const Value& v)
{
	std::string str;

	static const int BUFF_SZ = 500;
	char buff[BUFF_SZ];
	switch (v.GetType())
	{
		case IntVal			: snprintf(buff, BUFF_SZ, "%d", (int)v); str = buff; break;
		case FloatVal		: snprintf(buff, BUFF_SZ, "%f", (float)v); str = buff; break;
		case DoubleVal		: snprintf(buff, BUFF_SZ, "%f", (double)v); str = buff; break;
		case BoolVal		: str = v ? "true" : "false"; break;
		case NULLVal		: str = "null"; break;
		case ObjectVal		: str = Serialize(v); break;
		case ArrayVal		: str = SerializeArray(v); break;
		case StringVal		: str = std::string("\"") + (std::string)v + std::string("\""); break;
	}

	return str;
}

std::string SerializeArray(const Array& a)
{
	std::string str = "[";

	bool first = true;
	for (size_t i = 0; i < a.size(); i++)
	{
		const Value& v = a[i];
		if (!first)
			str += std::string(",");

		str += SerializeValue(v);

		first = false;
	}

	str += "]";
	return str;
}

std::string json::Serialize(const Value& v)
{
	std::string str;

	bool first = true;
	
	if (v.GetType() == ObjectVal)
	{
		str = "{";
		Object obj = v.ToObject();
		for (Object::ValueMap::const_iterator it = obj.begin(); it != obj.end(); it++)
		{
			if (!first)
				str += std::string(",");

			str += std::string("\"") + it->first + std::string("\":") + SerializeValue(it->second);
			first = false;
		}

		str += "}";
	}
	else if (v.GetType() == ArrayVal)
	{
		str = "[";
		Array a = v.ToArray();
		for (Array::ValueVector::const_iterator it = a.begin(); it != a.end(); it++)
		{
			if (!first)
				str += std::string(",");
			
			str += SerializeValue(*it);
			first = false;
		}
		
		str += "]";
			
	}
	// else it's not valid JSON, as a JSON data structure must be an array or an object. We'll return an empty string.
		
	
	return str;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
static Value DeserializeArray(std::string& str, std::stack<StackDepthType>& depth_stack);
static Value DeserializeObj(const std::string& _str, std::stack<StackDepthType>& depth_stack);

static Value DeserializeInternal(const std::string& _str, std::stack<StackDepthType>& depth_stack)
{
	Value v;
	
	std::string str = Trim(_str);
	if (str[0] == '{')
	{
		// Error: Began with a { but doesn't end with one
		if (str[str.length() - 1] != '}')
			return Value();
		
		depth_stack.push(InObject);
		v = DeserializeObj(str, depth_stack);
		if ((v.GetType() == NULLVal) || (depth_stack.top() != InObject))
			return v;
		
		depth_stack.pop();
	}
	else if (str[0] == '[')
	{
		// Error: Began with a [ but doesn't end with one
		if (str[str.length() - 1] != ']')
			return Value();
		
		depth_stack.push(InArray);
		v = DeserializeArray(str, depth_stack);
		if ((v.GetType() == NULLVal) || (depth_stack.top() != InArray))
			return v;
		
		depth_stack.pop();
	}
	else
	{
		// Will never get here unless _str is not valid JSON
		return Value();
	}
	
	return v;
}

static size_t GetEndOfArrayOrObj(const std::string& str, std::stack<StackDepthType>& depth_stack)
{
	size_t i = 1;
	bool in_quote = false;
	size_t original_count = depth_stack.size();
	
	for (; i < str.length(); i++)
	{
		if (str[i] == '\"')
		{
			if (str[i - 1] != '\\')
				in_quote = !in_quote;
		}
		else if (!in_quote)
		{
			if (str[i] == '[')
				depth_stack.push(InArray);
			else if (str[i] == '{')
				depth_stack.push(InObject);
			else if (str[i] == ']')
			{
				StackDepthType t = depth_stack.top();
				if (t != InArray)
				{
					// expected to be closing an array but instead we're inside an object block.
					// Example problem: {]}
					return std::string::npos;
				}
				
				size_t count = depth_stack.size();
				depth_stack.pop();
				if (count == original_count)
					break;
			}
			else if (str[i] == '}')
			{
				StackDepthType t = depth_stack.top();
				if (t != InObject)
				{
					// expected to be closing an object but instead we're inside an array.
					// Example problem: [}]
					return std::string::npos;
				}
					
				size_t count = depth_stack.size();
				depth_stack.pop();
				if (count == original_count)
					break;
			}
		}
	}
	
	return i;
}

bool appendasUTF8(unsigned code_point,std::string& str)
    {
#ifdef _WIN32  //defined for x86 and x64
    wchar_t strUtf16[4]=L"";
//    wchar_t strUtf16Normalized[16]=L"";
    CopyMemory(strUtf16,&code_point,4);
    ZeroMemory(strUtf16+2,2);
    char strUtf8[64]="";
//    int cbDstLength=sizeof(strUtf16Normalized)/sizeof(wchar_t);
//    NormalizeString(_NORM_FORM::NormalizationC,strUtf16,_tcslen(strUtf16),strUtf16Normalized,&cbDstLength);
    if (WideCharToMultiByte(CP_UTF8, 0, strUtf16, -1, strUtf8, sizeof(strUtf8), NULL, NULL)<=0)
        return false;
    str+=strUtf8;
#else
    if (code_point < 0x80)
        {
        str.push_back(code_point);
        }
    else if (code_point <= 0x7FF)
        {
        str.push_back((code_point >> 6) + 0xC0);
        str.push_back((code_point & 0x3F) + 0x80);
        }
    else if (code_point <= 0xFFFF)
        {
        str.push_back((code_point >> 12) + 0xE0);
        str.push_back(((code_point >> 6) & 0x3F) + 0x80);
        str.push_back((code_point & 0x3F) + 0x80);
        }
    else if (code_point <= 0x10FFFF)
        {
        str.push_back((code_point >> 18) + 0xF0);
        str.push_back(((code_point >> 12) & 0x3F) + 0x80);
        str.push_back(((code_point >> 6) & 0x3F) + 0x80);
        str.push_back((code_point & 0x3F) + 0x80);
        }
    else
        {
        return false; //error("invalid code_point");
        }
#endif
    return true;
    }

static std::string UnescapeJSONString(const std::string& str)
{
	std::string s = "";
	
	for (std::string::size_type i = 0; i < str.length(); i++)
	{
		char c = str[i];
		if ((c == '\\') && (i + 1 < str.length()))
		{
			int skip_ahead = 1;
            int sizeConverted=0;
            wchar_t utf8code=0;
            int err=0;
			std::string hex_str;
            
			switch (str[i+1])
			{
				case '"' : 	s.push_back('\"'); break;
				case '\\': 	s.push_back('\\'); break;
				case '/' : 	s.push_back('/'); break;
				case 't' : 	s.push_back('\t'); break;
				case 'n' : 	s.push_back('\n'); break;
				case 'r' : 	s.push_back('\r'); break;
				case 'b' :	s.push_back('\b'); break;
				case 'f' : 	s.push_back('\f'); break;
				case 'u' : 	skip_ahead = 5;
					hex_str = str.substr(i + 2, 4);
					utf8code = static_cast<wchar_t>(std::strtoul(hex_str.c_str(), nullptr, 16));
                    if(!appendasUTF8(utf8code,s))    
                        s+="?";
                    break;
					
				default: break;
			}
			
			i += skip_ahead;
		}
		else
			s.push_back(c);
	}
	
	return Trim(s);
}

static Value DeserializeValue(std::string& str, bool* had_error, std::stack<StackDepthType>& depth_stack)
{
	Value v;

	*had_error = false;
	str = Trim(str);

	if (str.length() == 0)
		return v;

	if (str[0] == '[')
	{
		// This value is an array, determine the end of it and then deserialize the array
		depth_stack.push(InArray);
		size_t i = GetEndOfArrayOrObj(str, depth_stack);
		if (i == std::string::npos)
		{
			*had_error = true;
			return Value();
		}
		
		std::string array_str = str.substr(0, i + 1);
		v = Value(DeserializeArray(array_str, depth_stack));
		str = str.substr(i + 1, str.length());
	}
	else if (str[0] == '{')
	{
		// This value is an object, determine the end of it and then deserialize the object
		depth_stack.push(InObject);
		size_t i = GetEndOfArrayOrObj(str, depth_stack);

		if (i == std::string::npos)
		{
			*had_error = true;
			return Value();
		}

		std::string obj_str = str.substr(0, i + 1);
		v = Value(DeserializeInternal(obj_str, depth_stack));
		str = str.substr(i + 1, str.length());
	}
	else if (str[0] == '\"')
	{
		// This value is a string
		size_t end_quote = GetQuotePos(str, 1);
		if (end_quote == std::string::npos)
		{
			*had_error = true;
			return Value();
		}

		v = Value(UnescapeJSONString(str.substr(1, end_quote - 1)));
		str = str.substr(end_quote + 1, str.length());
	}
	else
	{
		// it's not an object, string, or array so it's either a boolean or a number or null.
		// Numbers can contain an exponent indicator ('e') or a decimal point.
		bool has_dot = false;
		bool has_e = false;
		std::string temp_val;
		size_t i = 0;
		for (; i < str.length(); i++)
		{
			if (str[i] == '.')
				has_dot = true;
			else if ((str[i] == 'e') || (str[i] == 'E'))
				has_e = true;
			else if (str[i] == ']')
			{
				if (depth_stack.top() != InArray)
				{
					*had_error = true;
					return Value();
				}
				
				depth_stack.pop();
			}
			else if (str[i] == '}')
			{
				if (depth_stack.top() != InObject)
				{
					*had_error = true;
					return Value();
				}
				
				depth_stack.pop();
			}
			else if (str[i] == ',')
				break;

			if (!std::isspace(str[i]))
				temp_val += str[i];
		}

		// store all floating point as doubles. This will also set the float and int values as well.
		if (_stricmp(temp_val.c_str(), "true") == 0)
			v = Value(true);
		else if (_stricmp(temp_val.c_str(), "false") == 0)
			v = Value(false);
		else if (has_e || has_dot)
			v = Value(atof(temp_val.c_str()));
		else if (_stricmp(temp_val.c_str(), "null") == 0)
			v = Value();
		else
		{
			// Check if the value is beyond the size of an int and if so, store it as a double
			double tmp_val = atof(temp_val.c_str());
			if ((tmp_val >= (double)INT_MIN) && (tmp_val <= (double)INT_MAX))
				v = Value(atoi(temp_val.c_str()));
			else
				v = Value(tmp_val);
		}

		str = str.substr(i, str.length());
	}

	return v;
}

static Value DeserializeArray(std::string& str, std::stack<StackDepthType>& depth_stack)
{
	Array a;
	bool had_error = false;

	str = Trim(str);

	// Arrays begin and end with [], so if we don't find one, it's an error
	if ((str[0] == '[') && (str[str.length() - 1] == ']'))
		str = str.substr(1, str.length() - 2);
	else
		return Value();

	// extract out all values from the array (remember, a value can also be an array or an object)
	while (str.length() > 0)
	{
		std::string tmp;

		size_t i = 0;
		for (; i < str.length(); i++)
		{
			// If we get to an object or array, parse it:
			if ((str[i] == '{') || (str[i] == '['))
			{
				Value v = DeserializeValue(str, &had_error, depth_stack);
				if (had_error)
					return Value();
				
				if (v.GetType() != NULLVal)
					a.push_back(v);

				break;
			}

			bool terminate_parsing = false;

			if ((str[i] == ',') || (str[i] == ']'))
				terminate_parsing = true;			// hit the end of a value, parse it in the next block
			else
			{
				// keep grabbing chars to build up the value
				tmp += str[i];
				if  (i == str.length() - 1)
					terminate_parsing = true; // end of string, finish parsing
			}

			if (terminate_parsing)
			{
				Value v = DeserializeValue(tmp, &had_error, depth_stack);
				if (had_error)
					return Value();
				
				if (v.GetType() != NULLVal)
					a.push_back(v);

				str = str.substr(i + 1, str.length());
				break;
			}
		}
	}

	return a;
}

static Value DeserializeObj(const std::string& _str, std::stack<StackDepthType>& depth_stack)
{
	Object obj;

	std::string str = Trim(_str);

	// Objects begin and end with {} so if we don't find a pair, it's an error
	if ((str[0] != '{') && (str[str.length() - 1] != '}'))
		return Value();
	else
		str = str.substr(1, str.length() - 2);

	// Get all key/value pairs in this object...
	while (str.length() > 0)
	{
		// Get the key name
		size_t start_quote_idx = GetQuotePos(str);
		size_t end_quote_idx = GetQuotePos(str, start_quote_idx + 1);
		size_t colon_idx = str.find(':', end_quote_idx);

		if ((start_quote_idx == std::string::npos) || (end_quote_idx == std::string::npos) || (colon_idx == std::string::npos))
			return Value();	// can't find key name
		
		std::string key = str.substr(start_quote_idx + 1, end_quote_idx - start_quote_idx - 1);
		if (key.length() == 0)
			return Value();

		bool had_error = false;
		str = str.substr(colon_idx + 1, str.length());

		// We have the key, now extract the value from the string
		obj[key] = DeserializeValue(str, &had_error, depth_stack);
		if (had_error)
			return Value();
	}

	return obj;
}

Value json::Deserialize(const std::string &str)
{
	std::stack<StackDepthType> depth_stack;
	return DeserializeInternal(str, depth_stack);
}

