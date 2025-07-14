
//si ca marche pas verifier la valeur de PI

#ifndef DefSPGArg1
#error Argument DefSPGArg1 non defini (SPG_Argum_Inline)
#endif
#ifndef DefSPGArg2
#error Argument DefSPGArg2 non defini (SPG_Argum_Inline)
#endif

//sort entre 0 inclus et 2PI non inclus

{
SPG_COMPLEX SPG_I_C=DefSPGArg1;
    if (SPG_I_C.im > 0)
	{
        DefSPGArg2 = (float) (-atan(SPG_I_C.re / SPG_I_C.im) + PI * 0.5f);
	}
    else if (SPG_I_C.im < 0)
	{
        DefSPGArg2 = (float) (-atan(SPG_I_C.re / SPG_I_C.im) + PI * 1.5f);
	}
	else
	{
		if (SPG_I_C.re<0)
		{
        DefSPGArg2 = (float) PI;
		}
		else
		{
        DefSPGArg2 = 0;
		}
	}
}

#undef DefSPGArg1
#undef DefSPGArg2

