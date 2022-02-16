import sys


def main():

	operations = {
		"add": lambda a, b: a + b,
		"substract": lambda a, b: a - b,
		"divide": lambda a, b: a / b
	}
	
	result = ""
	
	if len(sys.argv) == 1:
		result = "Unspecified method !"
	elif len(sys.argv) == 2:
		result = "Missing 2 arguments !"
	elif len(sys.argv) == 3:
		result = "Missing 1 argument !"
	else:
		if sys.argv[1] in operations:
			try:
				result = f"the result is {operations[sys.argv[1]](int(sys.argv[2]), int(sys.argv[3]))}"
			except:
				result = "Error: at least one given argument is not a number !"
		else:
			result = "Error: undefined operation !"
			
	print(f"<!DOCTYPE html><html><body>{result}</body></html>")
	
	
main()