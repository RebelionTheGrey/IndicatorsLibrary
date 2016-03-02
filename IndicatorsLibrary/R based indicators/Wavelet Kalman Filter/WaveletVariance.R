WaveletMODWTVariance <- function(data, waveletType, highFreqLvl, lowFreqLvl)
{
    library(wmtsa);

    windowSize <- length(data);

    maxDecompositionLvl <- ilogb(windowSize, base = 2);
    transformedData <- wavMODWT(data, keep.series = FALSE, wavelet = waveletType);
    waveletVarianceLevels <- summary(transformedData);

    varianceDecomposition <- waveletVarianceLevels$smat[1:(maxDecompositionLvl + 1), 8];

    WaveletMODWTVariance <- c(sum(varianceDecomposition[1:highFreqLvl]), sum(varianceDecomposition[(maxDecompositionLvl + 1 - (lowFreqLvl - 1)):(maxDecompositionLvl + 1)]));
    WaveletMODWTVariance
}