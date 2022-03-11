import sys


def main():

	operations = {
		"add": lambda a, b: a + b,
		"substract": lambda a, b: a - b,
		"multiply": lambda a, b: a * b
	}
	
	result = ""
	args = []
	
	for i in range(1, len(sys.argv)):
		if sys.argv[i] != "undefined":
			args.append(sys.argv[i])
	
	if len(args) == 0:
		result = "Unspecified method !"
	elif len(args) == 1:
		result = "Missing 2 arguments !"
	elif len(args) == 2:
		result = "Missing 1 argument !"
	else:
		if args[0] in operations:
			try:
				result = f"the result is {operations[args[0]](int(args[1]), int(args[2]))}"
			except:
				result = "Error: at least one given argument is not a number !"
		else:
			result = "Error: undefined operation !"
			
	print(f"<!DOCTYPE html><html><body>{result}</body></html>")
	
	
main()