//----------------------------------------------------------------------------------------
//	Copyright © 2003 - 2011 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class simulates the behavior of the classic VB 'IsNumeric' function.
//----------------------------------------------------------------------------------------
public static class SimulateIsNumeric
{
	public static bool IsNumeric(object expression)
	{
		if (expression == null)
			return false;

		double testDouble;
		if (double.TryParse(expression.ToString(), out testDouble))
			return true;

		//VB's 'IsNumeric' returns true for any boolean value:
		bool testBool;
		if (bool.TryParse(expression.ToString(), out testBool))
			return true;

		return false;
	}
}