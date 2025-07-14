
#ifdef SC_FIXED_ARRAY
#error double include
#endif

#include "../SrcC/SPG_Global.h"

#if 1 //_DEBUG
// debug

template<typename T> 
class sc_checked_array
{
protected:
    const char* function; // caller
    const char* name; // array name
    int bounds; // no man's land size
    int size; // data size
    T* data; // bounds + size + bounds
    T* data_start; // size (point into data at end of 1st no man's land)
    
    char checksum_on_init;
    T invalid_reference;

	void on_fixed_array_overrun()
	{
		char msg[256];
		_snprintf(msg,256,"Function %.64s internal error on %.64s", function, name);
		 if(Global.EnableList>=1) MessageBox( 0, fdwstring(msg), L"sc_checked_array", MB_OK );
	
	}

    bool validate( int& index )
    {
        if( ( index >= 0 ) && ( index < size ) ) return true;
        if( index < 0 ) 
        {
            on_fixed_array_overrun();
            index = 0;
        }
        if( index >= size ) 
        {
            on_fixed_array_overrun();
            index = size-1;
        }
        return false;
    }

    char compute_sum() // no man's land uninit checksum
    {
        char checksum = 0;
        for(int i = 0; i < bounds * (signed)sizeof(T); i++)
        {
            checksum+=((char*)data)[i];
            checksum+=((char*)(data + bounds + size))[i];
        }
        return checksum;
    }

    bool check_sum()
    {
        char checksum = compute_sum();
        if( checksum != checksum_on_init ) { on_fixed_array_overrun(); return false; }
        else return true;
    }
public:
	sc_checked_array(T* data, int bounds, int size, const char* function, const char* name) : data(data), bounds(bounds), data_start(data+bounds), size(size), function(function), name(name)
	{
        checksum_on_init = compute_sum();
	}
    // ~sc_checked_array();
    T& operator[]( int index ) { check_sum(); validate( index ); return data_start[index]; };
    operator T*() { check_sum(); return data_start; };
};


template<typename T> 
class sc_fixed_array : public sc_checked_array<T> // emulates a stack-local static array
{
    static const int bounds = 128;
public:
    sc_fixed_array(int size, const char* function, const char* name) : sc_checked_array<T>(new T[size+2*bounds], bounds, size, function, name)
    {
    }
    ~sc_fixed_array()
    {
        check_sum();
        delete[] data;
    }
};

template<typename T> 
class sc_cached_array : public sc_checked_array<T> // emulates a pointer received as function argument. caches the actual data in a dynamically allocated array with no man's land bounds, copies the data back on destruction
{
    static const int bounds = 128;
    T* origin; // original argument
public:
    sc_cached_array(T* origin, int size, const char* function, const char* name) : origin(origin), sc_checked_array<T>(new T[size+2*bounds], bounds, size, function, name)
    {
        memcpy(data_start, origin, size*sizeof(T)); // copy from input array
    }
    ~sc_cached_array()
    {
        check_sum();
        memcpy(origin, data_start, size*sizeof(T)); // copy data back after use
        delete[] data;
    }
};

// instantiation

// debug
#define SC_FIXED_ARRAY( type, var, size ) sc_fixed_array<type> var(size, __FUNCTION__, #type " " #var "[" #size "]" )
#define SC_CACHED_ARRAY( type, alias, origin, size ) sc_cached_array<type> alias(origin, size, __FUNCTION__, #type "* " #alias " = " #origin "[" #size "]" )

#else
//release

//release
// stack-allocated static array
#define SC_FIXED_ARRAY( type, var, size ) type var[size]
// alias on a pointer received as function argument
#define SC_CACHED_ARRAY( type, alias, origin, size ) type*& alias = origin

#endif



