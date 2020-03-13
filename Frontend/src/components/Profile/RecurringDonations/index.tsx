import React from "react";
import { IUser } from "../../../api/types";
import { Card, ListItem, ListItemAvatar, Avatar, List, CardContent, ListItemText, ListItemSecondaryAction, IconButton } from "@material-ui/core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface IRecurringDonationProps {
    user: IUser;
}

export default ({ user }: IRecurringDonationProps) => {
    return <Card>
        <CardContent>
            <h3>Donation Subscriptions</h3>
            { user?.donationSubscriptions.length > 0 ?
                <List>
                {
                    user.donationSubscriptions.map(subscription =>
                    {
                    return <ListItem>
                        <ListItemAvatar>
                        <Avatar>
                            <FontAwesomeIcon icon="euro-sign" />
                        </Avatar>
                        </ListItemAvatar>
                        <ListItemText primary={`Recurring donation started at ${subscription.startDate}`} />
                        <ListItemSecondaryAction>
                        <IconButton>
                            <FontAwesomeIcon icon="trash" />
                        </IconButton>
                        </ListItemSecondaryAction>
                    </ListItem>;
                    })
                }
                </List> : 
                <p>You have no recurring donations</p>
            }
        </CardContent>
    </Card>;
};