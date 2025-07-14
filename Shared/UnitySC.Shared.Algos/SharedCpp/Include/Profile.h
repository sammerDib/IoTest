#pragma once
#include "Point2d.h"

class ExclusionZone;

#pragma unmanaged

/// <summary>
/// This class holds a list of 2D points, ordered by X coordinate.
/// For example it is use to manage Profile 1D scan results (for different kind of measurements, Step, Trench...)
/// </summary>
class Profile
{
public:
    constexpr Profile() = default;

    /// <summary>
    /// Creates a Profile, holding the given list of points.
    /// points must be ordered by X coordinate.
    /// </summary>
    /// <param name="points"></param>
    Profile(const std::vector<geometry::Point2d>& points) : _points(points)
    {
    }

    const std::vector<geometry::Point2d>& GetPoints() const;
    
    /// <summary>
    /// Add point to the end of Profile.
    /// point.X must be greater than all other points' X already in this Profile
    /// </summary>
    /// <param name="point">The point to add</param>
    void PushBack(const geometry::Point2d& point);
    /// <summary>
    /// Reserve size points in the underlying container.
    /// </summary>
    /// <param name="size">Number of points that the Profile can contain without having to perform a resize.</param>
    void Reserve(const int size);
    /// <summary>
    /// Returns the number of points in the profile
    /// </summary>
    /// <returns>the number of points in the profile</returns>
    size_t Size() const;
    /// <summary>
    /// Removes the last point
    /// </summary>
    void PopBack();
    /// <summary>
    /// Gives the first point of the profile
    /// </summary>
    /// <returns>the first point of the profile</returns>
    geometry::Point2d Front() const;
    /// <summary>
    /// Gives the last point of the profile
    /// </summary>
    /// <returns>the last point of the profile</returns>
    geometry::Point2d Back() const;
    /// <summary>
    /// Reverses Y coordinates. Keep X coordinates in the same order.
    /// </summary>
    void Reverse();

    /// <summary>
    /// Concatenate this profile and the one passed in parameter
    /// </summary>
    /// <param name="toAppend">The profile to append to this profile</param>
    void Concat(const Profile& toAppend);

    /// <summary>
    /// Remove points which have X coordinate inside the range exclusionZone.
    /// </summary>
    /// <param name="exclusionZone">Ranges of X values to remove</param>
    void RemoveExclusionZone(const ExclusionZone& exclusionZone);
    /// <summary>
    /// Remove points that have a Y coordinate at a distance of the average > nbStdDevFiltering * std dev 
    /// </summary>
    /// <param name="average">The average of this profile</param>
    /// <param name="stdDev">The standard deviation of this profile</param>
    /// <param name="nbStdDevFiltering">How wide is the removal</param>
    void RemoveStdDevY(const double average, const double stdDev, const double nbStdDevFiltering);
    /// <summary>
    /// Remove points that have a Y coordinate = NaN 
    /// </summary>
    void FilterNanY();

    /// <summary>
    /// The goal of this struct is to provide an iterator to iterate only on X or Y values.
    /// This iterator can be used with standard library functions.
    /// Most of the time use of begin_x(), begin_y(), end_x(), end_y() should be enough.
    /// </summary>
    /// <typeparam name="isConst">True to have a constant iterator (i.e. iterator to a const double), false for a mutable one.</typeparam>
    template <bool isConst>
    struct iterator
    {
        using iterator_category = std::random_access_iterator_tag;
        using difference_type = std::ptrdiff_t;
        using value_type = std::conditional_t<isConst, const double, double>;
        using pointer = value_type*;
        using reference = value_type&;

        iterator(pointer ptr) : _ptr(ptr) {}

        reference operator*() const { return *_ptr; }
        pointer operator->() { return _ptr; }

        // Prefix increment
        iterator& operator++() { _ptr += 2; return *this; }

        // Postfix increment
        iterator operator++(int) { iterator tmp = *this; ++(*this); return tmp; }

        iterator& operator+=(const int n)
        {
            this->_ptr += 2 * n;
            return *this;
        }

        iterator& operator-=(const int n)
        {
            this->_ptr -= 2 * n;
            return *this;
        }

        friend bool operator== (const iterator& a, const iterator& b) { return a._ptr == b._ptr; };
        friend bool operator!= (const iterator& a, const iterator& b) { return a._ptr != b._ptr; };

        friend iterator operator+(iterator lhs, const int rhs)
        {
            lhs += rhs;
            return lhs;
        }

        friend iterator operator-(iterator lhs, const int rhs)
        {
            lhs -= rhs;
            return lhs;
        }

        friend int operator-(iterator lhs, const iterator& rhs)
        {
            return (lhs._ptr - rhs._ptr) / 2;
        }

    private:
        pointer _ptr;
    };

    using Profile_Iterator = iterator<false>;
    using Profile_ConstIterator = iterator<true>;

    Profile_Iterator begin_x();
    Profile_Iterator end_x();
    Profile_ConstIterator begin_x() const;
    Profile_ConstIterator end_x() const;

    Profile_Iterator begin_y();
    Profile_Iterator end_y();
    Profile_ConstIterator begin_y() const;
    Profile_ConstIterator end_y() const;

    using PointsIterator = std::vector<geometry::Point2d>::iterator;
    using ConstPointsIterator = std::vector<geometry::Point2d>::const_iterator;

    PointsIterator begin_point();
    PointsIterator end_point();
    ConstPointsIterator begin_point() const;
    ConstPointsIterator end_point() const;

    struct CompareX
    {
        bool operator()(const geometry::Point2d& p, const double v) const { return p.X < v; }
        bool operator()(const double v, const geometry::Point2d& p) const { return v < p.X; }
    };

    bool operator==(const Profile& rhs) const;

private:
    std::vector<geometry::Point2d> _points;
};
