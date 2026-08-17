import json
import os
import urllib.request
import boto3

secrets = boto3.client("secretsmanager")

SECRET_NAME = os.environ["BREVO_SECRET_NAME"]


def get_brevo_config():
    response = secrets.get_secret_value(
        SecretId=SECRET_NAME
    )

    return json.loads(response["SecretString"])


def send_email(config, notification):

    payload = {
        "sender": {
            "name": config["fromName"],
            "email": config["fromEmail"]
        },
        "to": [
            {
                "email": notification["recipientEmail"],
                "name": notification["recipientName"]
            }
        ],
        "subject": (
            f"[Ticket {notification['ticketNumber']}] "
            "New Comment Added"
        ),
        "textContent": (
            f"Hello {notification['recipientName']},\n\n"
            f"A new comment was added to ticket "
            f"{notification['ticketNumber']}.\n\n"
            f"Title: {notification['ticketTitle']}\n\n"
            f"Commented by: {notification['commenterName']}\n\n"
            f"Comment:\n{notification['comment']}\n\n"
            "Regards,\n"
            "Ticket Management System"
        )
    }

    request = urllib.request.Request(
        "https://api.brevo.com/v3/smtp/email",
        data=json.dumps(payload).encode("utf-8"),
        headers={
            "accept": "application/json",
            "api-key": config["apiKey"],
            "content-type": "application/json"
        },
        method="POST"
    )

    with urllib.request.urlopen(request, timeout=15) as response:
        return response.read().decode("utf-8")


def lambda_handler(event, context):

    config = get_brevo_config()

    for record in event["Records"]:

        notification = json.loads(record["body"])

        print(
            f"Sending email to "
            f"{notification['recipientEmail']}"
        )

        result = send_email(config, notification)

        print(f"Brevo response: {result}")

    return {
        "statusCode": 200,
        "body": "Email notification processed"
    }
