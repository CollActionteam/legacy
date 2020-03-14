import React from "react";
import { IUser } from "../../../api/types";
import { Card, List, CardContent } from "@material-ui/core";
import RecurringDonationSubscription from "../RecurringDonationSubscription";

interface IRecurringDonationProps {
    user: IUser;
    setUser(user: IUser | null): void;
}

export default ({ user, setUser }: IRecurringDonationProps) => {
    return <Card>
        <CardContent>
            <h3>Donation Subscriptions</h3>
            { user?.donationSubscriptions.length > 0 ?
                <List>
                { user.donationSubscriptions.map(subscription => <RecurringDonationSubscription user={user} subscription={subscription} setUser={setUser} />) }
                </List> : 
                <p>You have no recurring donations</p>
            }
        </CardContent>
    </Card>;
};