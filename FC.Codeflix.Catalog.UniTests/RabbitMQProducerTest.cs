﻿using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.Producer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests
{
    public class RabbitMQProducerTest
    {
        [Fact]
        [Trait("Application", "RabbitMQProducer")]
        public async Task SendMessage()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "adm_videos",
                Password = "123456"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ConfirmSelect();
            var options = Options.Create(new RabbitMQConfiguration
            {
                Exchange = "video.events"
            }); ;
            var producer = new RabbitMQProducer(channel, options);
            var @event = new VideoUploadedEvent(Guid.NewGuid(), "videos/test.mp4");
            await producer.SendMessageAsync(@event, CancellationToken.None);
        }
    }
}
