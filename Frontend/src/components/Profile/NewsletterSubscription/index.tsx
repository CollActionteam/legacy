import React from "react";
import { Card, CardContent, CardActions, Button } from "@material-ui/core";
import { IUser } from "../../../api/types";

interface INewsletterSubscriptionProps {
    user: IUser;
}

export default ({ user }: INewsletterSubscriptionProps) => {
    return <Card>
        <CardContent>
            <h3>Newsletter subscription</h3>
            {
                user.isSubscribedNewsletter ? 
                    <p>Unsubscribe from our newsletter, we'll be sad to see you go!</p> : 
                    <p>Subscribe to our newsletter if you would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! 🙂</p>
            }
        </CardContent>
        <CardActions>
            {
                user.isSubscribedNewsletter ? 
                    <Button>Unsubscribe</Button> : 
                    <Button>Subscribe</Button>
            }
        </CardActions>
    </Card>
};