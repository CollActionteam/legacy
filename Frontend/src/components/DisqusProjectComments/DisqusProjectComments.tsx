import { DiscussionEmbed } from 'disqus-react';
import { IProject } from '../../api/types';
import React from 'react';
import { useConsent } from '../../providers/CookieConsentProvider';
import { useSettings } from '../../providers/SettingsProvider';

export default ({ project }: { project: IProject }) => {
    const disqusId = `${project.name}/${project.id}`;
    const { disqusSiteId } = useSettings();
    const { consent } = useConsent();
    if (!consent.includes("disqus")) {
        return null;
    } else {
        return <DiscussionEmbed shortname={disqusSiteId} config={
        {
            url: window.location.href,
            identifier: disqusId,
            title: `${project.name} discussion`
        }} />;
    }
};