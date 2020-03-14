import React, { useState } from "react";
import { Card, CardContent, CardActions, Button } from "@material-ui/core";
import { IUser } from "../../../api/types";
import { useMutation, gql } from "@apollo/client";
import { Alert } from "../../Alert";

interface INewsletterSubscriptionProps {
    user: IUser;
}

export default ({ user }: INewsletterSubscriptionProps) => {
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [toggleSubscription] =
        useMutation(
            UPDATE_USER,
            {
                variables: {
                    updatedUser: {
                        id: user.id,
                        email: user.email,
                        firstName: user.firstName,
                        lastName: user.lastName,
                        isSubscribedNewsletter: !user.isSubscribedNewsletter
                    }
                },
                onCompleted: (data) => {
                    if (data.applicationUser.updateUser.result.succeeded) {
                        user.isSubscribedNewsletter = !user.isSubscribedNewsletter;
                    } else {
                        let error = data.applicationUser.updateUser.errors.map((e: any) => e.description).join(", ");
                        setErrorMessage(error);
                    }
                },
                onError: (data) => {
                    setErrorMessage(data.message);
                }
            });

    return <React.Fragment>
            { errorMessage ? <Alert type="error" text={errorMessage} /> : null }
            <Card>
                <CardContent>
                    <h3>Newsletter subscription</h3>
                    {
                        user.isSubscribedNewsletter ? 
                            <p>Unsubscribe from our newsletter, we'll be sad to see you go!</p> : 
                            <p>Subscribe to our newsletter if you would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! <span role="img" aria-label="smiley">🙂</span></p>
                    }
                </CardContent>
                <CardActions>
                    <Button onClick={() => toggleSubscription()}>{ user.isSubscribedNewsletter ? "Unsubscribe" : "Subscribe" }</Button>
                </CardActions>
            </Card>
        </React.Fragment>;
};

const UPDATE_USER = gql`
    mutation UpdateUser($updatedUser: UpdatedUserInputGraph!)
    {  
        applicationUser {
            updateUser(user:$updatedUser) {
                user {
                    id
                    isSubscribedNewsletter
                }
                result {
                    succeeded
                    errors {
                        code
                        description
                    }
                }
            }
        }
    }`;