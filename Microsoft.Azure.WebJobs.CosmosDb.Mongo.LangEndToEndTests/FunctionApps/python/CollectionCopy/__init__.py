import logging
from azure.functions import Out

def main(input : str, output: Out[str]):
    logging.info(input)
    output.set(input)
