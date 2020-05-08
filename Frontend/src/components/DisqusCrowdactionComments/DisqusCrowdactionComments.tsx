import { DiscussionEmbed } from 'disqus-react';
import { ICrowdaction } from '../../api/types';
import React from 'react';
import { useConsent, Consent } from '../../providers/CookieConsentProvider';
import { useSettings } from '../../providers/SettingsProvider';

export default ({ crowdaction }: { crowdaction: ICrowdaction }) => {
    const disqusId = `${crowdaction.name}/${crowdaction.id}`;
    const { disqusSiteId } = useSettings();
    const { consent } = useConsent();
    if (!consent.includes(Consent.Disqus) || !disqusSiteId) {
        return null;
    } else {
        return <DiscussionEmbed shortname={disqusSiteId} config={
        {
            url: window.location.href,
            identifier: disqusId,
            title: `${crowdaction.name} discussion`
        }} />;
    }
};