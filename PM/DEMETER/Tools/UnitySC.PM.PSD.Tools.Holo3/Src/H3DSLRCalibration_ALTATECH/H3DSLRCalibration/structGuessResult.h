struct strGuessResult{
	H3_ARRAY_PT2DFLT32 Points;
	H3_ARRAY_INT8      Sgn;
	
	strGuessResult& operator=(const strGuessResult& GR){
		if (this==&GR) return *this;
			
		Points=GR.Points;
		Sgn=GR.Sgn;

		return *this;
	}
};