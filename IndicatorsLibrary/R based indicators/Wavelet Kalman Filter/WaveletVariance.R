WaveletMODWTVariance <- function(data, waveletType)
{
    library(wmtsa);

    windowSize <- length(data);

    maxDecompositionLvl <- ilogb(windowSize, base = 2);
    transformedData <- wavMODWT(data, keep.series = FALSE, wavelet = waveletType);
    waveletVarianceLevels <- summary(transformedData);

    WaveletMODWTVariance <- waveletVarianceLevels$smat[1:(maxDecompositionLvl + 1), 8];
    WaveletMODWTVariance
}