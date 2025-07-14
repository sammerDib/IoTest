#pragma once

#include "Point2d.h"
#include "Profile.h"

#pragma managed
using namespace System::Collections::Generic;

namespace UnitySCSharedAlgosCppWrapper {
    public ref class Profile2d : public IEnumerable<Point2d^>
    {
    public:
        Profile2d();
        ~Profile2d();
        !Profile2d();

        void Add(Point2d^ point);

        const Profile& GetNative();

        property Point2d^ default[int]
        {
            Point2d^ get(int index)
            {
                if (index < 0 || index >= _native->Size())
                {
                    throw gcnew System::IndexOutOfRangeException("Index was outside the bounds of the Profile2d.");
                }
                auto& nativePoint = *std::next(_native->begin_point(), index);
                return gcnew Point2d(nativePoint);
            }
            void set(int index, Point2d^ value)
            {
                if (index < 0 || index >= _native->Size())
                {
                    throw gcnew System::IndexOutOfRangeException("Index was outside the bounds of the Profile2d.");
                }
                *std::next(_native->begin_point(), index) = geometry::Point2d(value->X, value->Y);
            }
        }

        ref struct enumerator : IEnumerator<Point2d^>
        {
            enumerator(Profile2d^ myArr)
            {
                collection = myArr;
                currentIndex = -1;
            }

            virtual bool MoveNext() = IEnumerator<Point2d^>::MoveNext
            {
                if (currentIndex < static_cast<int>(collection->_native->Size() - 1) )
                {
                    ++currentIndex;
                    return true;
                }
                return false;
            }

            property Point2d^ Current
            {
                virtual Point2d^ get() = IEnumerator<Point2d^>::Current::get
                {
                    auto& nativePoint = *std::next(collection->_native->begin_point(), currentIndex);
                    return gcnew Point2d(nativePoint);
                }
            };
            // This is required as IEnumerator<T> also implements IEnumerator
            property Object^ Current2
            {
                virtual Object^ get() = System::Collections::IEnumerator::Current::get
                {
                    auto& nativePoint = *std::next(collection->_native->begin_point(), currentIndex);
                    return gcnew Point2d(nativePoint);
                }
            };

            virtual void Reset() = IEnumerator<Point2d^>::Reset{}
            ~enumerator() {}

            Profile2d^ collection;
            int currentIndex;
        };

        virtual IEnumerator<Point2d^>^ GetEnumerator()
        {
            return gcnew enumerator(this);
        }

        virtual System::Collections::IEnumerator^ GetEnumerator2() = System::Collections::IEnumerable::GetEnumerator
        {
            return gcnew enumerator(this);
        }

    private:
        Profile* _native;
    };
}
