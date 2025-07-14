#include "pch.h"
#include "Profile.h"

#include "ExclusionZone.h"

const std::vector<geometry::Point2d>& Profile::GetPoints() const
{
    return _points;
}
void Profile::PushBack(const geometry::Point2d& point)
{
    _points.push_back(point);
}
void Profile::Reserve(const int size)
{
    _points.reserve(size);
}
size_t Profile::Size() const
{
    return _points.size();
}
void Profile::PopBack()
{
    _points.pop_back();
}
geometry::Point2d Profile::Front() const
{
    return _points.front();
}
geometry::Point2d Profile::Back() const
{
    return _points.back();
}
void Profile::Reverse()
{
    for (size_t i = 0, j = _points.size() - 1; i < j; ++i, --j) 
    {
        auto temp = _points[i];
        _points[i].Y = _points[j].Y;
        _points[j].Y = temp.Y;
    }
}

void Profile::Concat(const Profile& toAppend)
{
    _points.insert(_points.end(), toAppend.begin_point(), toAppend.end_point());
}

void Profile::RemoveExclusionZone(const ExclusionZone& exclusionZone)
{
    // These 2 points are useful if exclusion zone is only right or left.
    const auto [leftPointIt, rightPointIt] =
        std::equal_range(_points.begin(), _points.end(), exclusionZone.GetX(), Profile::CompareX{});

    // Find the left (i.e. lowest x-wise) point to remove.
    auto leftLowerBound = leftPointIt;
    if (exclusionZone.GetLeft() > 0.0)
    {
        const auto leftBound = exclusionZone.GetX() - exclusionZone.GetLeft();
        leftLowerBound = std::lower_bound(
            _points.begin(), _points.end(), leftBound, [](const geometry::Point2d& point, double value)
            {
                return point.X < value;
            });
        if (leftLowerBound == _points.end())
        {
            return;
        }
    }

    // Find the right (i.e. highest x-wise) point to remove.
    auto rightUpperBound = rightPointIt;
    if (exclusionZone.GetRight() > 0.0)
    {
        const auto rightBound = exclusionZone.GetX() + exclusionZone.GetRight();
        rightUpperBound = std::upper_bound(
            _points.begin(), _points.end(), rightBound, [](double value, const geometry::Point2d& point)
            {
                return value < point.X;
            });
    }

    _points.erase(leftLowerBound, rightUpperBound);
}

void Profile::RemoveStdDevY(const double average, const double stdDev, const double nbStdDevFiltering)
{
    _points.erase(std::remove_if(_points.begin(), _points.end(),
        [&average, &stdDev, &nbStdDevFiltering](const auto& point) {
            return point.Y < average - nbStdDevFiltering * stdDev || point.Y > average + nbStdDevFiltering * stdDev;
        }), _points.end());
}

void Profile::FilterNanY()
{
    _points.erase(std::remove_if(_points.begin(), _points.end(),
        [](const auto& xy) { return std::isnan(xy.Y); }),
        _points.end());
}

Profile::iterator<false> Profile::begin_x()
{ 
    return iterator<false>(&_points.front().X);
}
Profile::iterator<false> Profile::end_x()
{ 
    return iterator<false>(&(_points.data() + _points.size())->X);
}
Profile::iterator<true> Profile::begin_x() const
{
    return iterator<true>(&_points.front().X);
}
Profile::iterator<true> Profile::end_x() const
{
    return iterator<true>(&(_points.data() + _points.size())->X);
}

Profile::iterator<false> Profile::begin_y()
{ 
    return iterator<false>(&_points.front().Y);
}
Profile::iterator<false> Profile::end_y()
{ 
    return iterator<false>(&(_points.data() + _points.size())->Y);
}
Profile::iterator<true> Profile::begin_y() const
{
    return iterator<true>(&_points.front().Y);
}
Profile::iterator<true> Profile::end_y() const
{
    return iterator<true>(&(_points.data() + _points.size())->Y);
}

std::vector<geometry::Point2d>::const_iterator Profile::begin_point() const
{
    return _points.begin();
}
std::vector<geometry::Point2d>::const_iterator Profile::end_point() const
{
    return _points.end();
}
std::vector<geometry::Point2d>::iterator Profile::begin_point()
{
    return _points.begin();
}
std::vector<geometry::Point2d>::iterator Profile::end_point()
{
    return _points.end();
}

bool Profile::operator==(const Profile& rhs) const
{
    if (Size() != rhs.Size())
    {
        return false;
    }
    for (auto i = 0; i < Size(); ++i)
    {
        if (_points[i].X != rhs.GetPoints()[i].X || _points[i].Y != rhs.GetPoints()[i].Y)
        {
            return false;
        }
    }
    return true;
}
