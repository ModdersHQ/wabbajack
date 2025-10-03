using System;
using System.Threading;
using System.Threading.Tasks;
using Wabbajack.DTOs.DownloadStates;
using Wabbajack.DTOs.Interventions;

namespace Wabbajack.UserIntervention;

public class ManualDownloadHandler : BrowserWindowViewModel
{
    public ManualDownload Intervention { get; set; }

    public ManualDownloadHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    protected override async Task Run(CancellationToken token)
    {
        var uri = default(ManualDownload.BrowserDownloadState);
        try
        {
            var archive = Intervention.Archive;
            var md = Intervention.Archive.State as Manual;

            HeaderText = $"Manual download for {archive.Name} ({md.Url.Host})";

            Instructions = string.IsNullOrWhiteSpace(md.Prompt) ? $"Please download {archive.Name}" : md.Prompt;

            var task = WaitForDownloadUri(token, async () =>
            {
                await RunJavaScript("Array.from(document.getElementsByTagName(\"iframe\")).forEach(f => {if (f.title != \"SP Consent Message\" && !f.src.includes(\"challenges.cloudflare.com\")) f.remove()})");
            });
            await NavigateTo(md.Url);

            await RunJavaScript(@"
                (function() {
                    function findAndClickButton() {
                        // Helper function to search within shadow roots recursively
                        function findButtonInShadowDom(root) {
                            // First try to find by ID in this root
                            var button = root.getElementById ? root.getElementById('slowDownloadButton') : null;
                            if (button) return button;
                            
                            // Then search all buttons in this root
                            var buttons = root.querySelectorAll ? root.querySelectorAll('button') : [];
                            for (var i = 0; i < buttons.length; i++) {
                                var btn = buttons[i];
                                var text = (btn.innerText || btn.textContent || '').trim().toLowerCase();
                                if (text === 'slow download' || 
                                    (btn.querySelector && btn.querySelector('span') && 
                                     (btn.querySelector('span').textContent || '').trim().toLowerCase() === 'slow download')) {
                                    return btn;
                                }
                            }
                            
                            // Recursively search shadow roots
                            var allElements = root.querySelectorAll ? root.querySelectorAll('*') : [];
                            for (var j = 0; j < allElements.length; j++) {
                                if (allElements[j].shadowRoot) {
                                    var result = findButtonInShadowDom(allElements[j].shadowRoot);
                                    if (result) return result;
                                }
                            }
                            
                            return null;
                        }
                        
                        // Search in main document and all shadow roots
                        var slowButton = findButtonInShadowDom(document);
                        
                        if (!slowButton) {
                            console.log('Slow download button not found, retrying in 500ms');
                            setTimeout(findAndClickButton, 500);
                            return;
                        }
                        
                        // First make sure the button is visible
                        slowButton.scrollIntoView({ behavior: 'smooth', block: 'center' });
                        
                        // Force center the button in the viewport using calculated positions
                        var rect = slowButton.getBoundingClientRect();
                        var buttonMiddle = rect.top + rect.height / 2;
                        var viewportHeight = window.innerHeight;
                        var scrollY = window.scrollY + buttonMiddle - viewportHeight / 2;
                        window.scrollTo({ top: scrollY, behavior: 'smooth' });
                        console.log('Centering button at position:', scrollY);
                        
                        // Wait a moment for the scroll to complete
                        setTimeout(function() {
                            // Click the button
                            console.log('Clicking slow download button');
                            slowButton.click();
                            
                            // Close the window after clicking
                            setTimeout(function() {
                                window.close();
                            }, 2000);
                        }, 1000);
                    }
                    
                    // Start the process
                    findAndClickButton();
                })();");
                
            uri = await task;
        }
        finally
        {
            Intervention.Finish(uri);
        }
    }
}