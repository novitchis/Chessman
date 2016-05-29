#pragma once

#define _CRT_SECURE_NO_WARNINGS 1

// this is to fix the arm build
// don't know yet what disabling pre-fetching means
#define NO_PREFETCH 1

#include <collection.h>
#include <ppltasks.h>

#include <algorithm>
#include <cassert>
#include <ostream>

