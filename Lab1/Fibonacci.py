def fibonacci(n):
    if n < 0:
        raise ValueError("Input should be a non-negative integer.")
    elif n == 0:
        return 0
    elif n == 1:
        return 1
    else:
        a, b = 0, 1
        for _ in range(2, n + 1):
            a, b = b, a + b
        return b

if __name__ == "__main__":
    try:
        n = int(input("Enter a non-negative integer: "))
        result = fibonacci(n)
        print(f"The Fibonacci number at position {n} is: {result}")
    except ValueError as e:
        print(e)
