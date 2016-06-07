#include <string>
#include <map>
#include <fstream>
#include <sstream>
#include <iostream>

using namespace std;

class MoveSet
{
public:
	string eco;
	string pgn;

public:
	MoveSet() : eco("unknown"), pgn("")
	{
		/*	required for map retrieval
			the map operator[] inserts a new default entry
		*/
	}

	MoveSet(string eco, string pgn) : eco(eco), pgn(pgn)
	{
	}

	~MoveSet()
	{
	}
};

class MoveDatabase
{
private:
	map<string, MoveSet> db;

private:
	bool parseLine(string line)
	{
		string eco, pgn, name;
		istringstream scanner(line);

		getline(scanner, eco, ' ');
		scanner.seekg(1, scanner.cur);
		getline(scanner, name, '"');
		scanner.seekg(1, scanner.cur);

		// don't ask about the yoda pattern here
		if (-1 == scanner.tellg())
			return false;

		while(scanner.peek() == ' ')
		{
			scanner.seekg(1, scanner.cur);
		}

		getline(scanner, pgn, '\0');
		db[name] = MoveSet(eco, pgn);

		return true;
	}

public:
	MoveDatabase(string filename)
	{
		string line;
		ifstream f(filename);
		while (getline(f, line))
		{
			string newline;

			if (line.length() == 0 || line[0] == '#')
				continue;

			if (! parseLine(line))
			{
				// double line opening - just add another fckn line
				getline(f, newline);
				line += " " + newline;
				parseLine(line);
			}
				
		}
	}

	MoveSet operator[] (string name)
	{
		return db[name];
	}

	void serializeTo(string filename)
	{
		ofstream f(filename);

		for (auto entryIt = db.begin(); entryIt != db.end(); ++entryIt)
		{
			string name = entryIt->first;
			MoveSet moveSet = entryIt->second;

			f << moveSet.eco << " \"" << name << "\" " << moveSet.pgn << endl;
		}

		f.close();
	}

	~MoveDatabase()
	{
	}
};