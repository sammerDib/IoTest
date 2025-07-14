#include <opencv2/imgproc.hpp>

#include "RadonTransform.hpp"

#pragma unmanaged
namespace radon_transform
{
    void RadonTransform(cv::InputArray src, cv::OutputArray dst, double theta, double start_angle, double end_angle)
    {
        CV_Assert(src.dims() == 2);
        CV_Assert(src.channels() == 1);
        CV_Assert((end_angle - start_angle) * theta > 0);

        cv::Mat _srcMat = src.getMat();

        int _row_num, _col_num, _out_mat_type;
        _col_num = cvRound((end_angle - start_angle) / theta);
        transpose(_srcMat, _srcMat);
        cv::Mat _masked_src;
        cv::Point _center;

        if (_srcMat.type() == CV_32FC1 || _srcMat.type() == CV_64FC1) {
            _out_mat_type = CV_64FC1;
        }
        else {
            _out_mat_type = CV_32SC1;
        }

        // avoid cropping corner when rotating
        _row_num = cvCeil(sqrt(_srcMat.rows * _srcMat.rows + _srcMat.cols * _srcMat.cols));
        _masked_src = cv::Mat(cv::Size(_row_num, _row_num), _srcMat.type(), cv::Scalar(0));
        _center = cv::Point(_masked_src.cols / 2, _masked_src.rows / 2);
        _srcMat.copyTo(_masked_src(cv::Rect(
            (_row_num - _srcMat.cols) / 2,
            (_row_num - _srcMat.rows) / 2,
            _srcMat.cols, _srcMat.rows)));

        double _t;
        cv::Mat _rotated_src;
        cv::Mat _radon(_row_num, _col_num, _out_mat_type);

        for (int _col = 0; _col < _col_num; _col++) {
            // rotate the source by _t
            _t = (start_angle + _col * theta);
            cv::Mat _r_matrix = cv::getRotationMatrix2D(_center, _t, 1);
            cv::warpAffine(_masked_src, _rotated_src, _r_matrix, _masked_src.size());
            cv::Mat _col_mat = _radon.col(_col);
            // make projection
            cv::reduce(_rotated_src, _col_mat, 1, cv::REDUCE_SUM, _out_mat_type);
        }

        _radon.copyTo(dst);
        return;
    }
}