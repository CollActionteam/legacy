import { IDonationSubscription, IUser } from "../../../api/types";
import React, { useState } from "react";
import { ListItemAvatar, Avatar, ListItemText, ListItemSecondaryAction, IconButton, ListItem } from "@material-ui/core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useMutation } from "@apollo/client/react/hooks/useMutation";
import { gql } from "@apollo/client";
import { Alert } from "../../Alert";

interface IRecurringDonationItemProps {
    user: IUser;
    setUser(user: IUser | null): void;
    subscription: IDonationSubscription;
}

export default ({ user, setUser, subscription }: IRecurringDonationItemProps) => {
    const [ stopSubscription ] = useMutation(STOP_SUBSCRIPTION,
        {
            variables: {
                subscriptionId: subscription.id
            },
            onCompleted: (_) => {
                user.donationSubscriptions = user.donationSubscriptions.filter(sub => sub.id !== subscription.id);
                setUser(user);
            },
            onError: (data) => {
                setErrorMessage(data.message);
            }
        });
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    return <React.Fragment>
        { errorMessage ? <Alert type="error" text={errorMessage} /> : null }
        <ListItem>
            <ListItemAvatar>
            <Avatar>
                <FontAwesomeIcon icon="euro-sign" />
            </Avatar>
            </ListItemAvatar>
            <ListItemText primary={`Recurring donation started at ${subscription.startDate}`} />
            <ListItemSecondaryAction>
            <IconButton onClick={() => stopSubscription()}>
                <FontAwesomeIcon icon="trash" />
            </IconButton>
            </ListItemSecondaryAction>
        </ListItem>
    </React.Fragment>;
};

const STOP_SUBSCRIPTION = gql`
mutation StopSubscription($subscriptionId: ID!)
{  
  donation {
    cancelSubscription($subscriptionId)
  }
}`;